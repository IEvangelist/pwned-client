// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

internal static class HttpClientBuilderExtensions
{
    static readonly string[] s_redactedHttpHeaders =
    [
        HttpHeaderNames.HibpApiKey,
        nameof(HttpRequestHeader.Authorization),
        nameof(HttpRequestHeader.Cookie),
    ];

    internal static void ConfigureHttpResilience(
        this IHttpClientBuilder builder,
        Action<HttpStandardResilienceOptions>? configureResilienceOptions)
    {
        // Don't log our API key, or auth/cookie headers.
        builder.RedactLoggedHeaders(
            shouldRedactHeaderValue: static headerName => s_redactedHttpHeaders.Any(
                    predicate: name => string.Equals(
                            name, headerName, StringComparison.OrdinalIgnoreCase)));

        if (configureResilienceOptions is not null)
        {
            _ = builder.AddStandardResilienceHandler(configureResilienceOptions);
        }
        else
        {
            builder.AddStandardResilienceHandler(static options =>
            {
                // Configure rate limit handling for HIBP API
                // The API returns HTTP 429 with a retry-after header when rate limit is exceeded
                // See: https://haveibeenpwned.com/API/v3#RateLimiting
                
                // Configure retry policy to respect retry-after header from HIBP API
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.UseJitter = true;
                
                // Respect retry-after header when present (HTTP 429 responses)
                options.Retry.DelayGenerator = static args =>
                {
                    // Check if the response has a retry-after header
                    if (args.Outcome.Result?.Headers.RetryAfter is { } retryAfter)
                    {
                        // Use the retry-after value from the API
                        var delay = retryAfter.Delta ?? (retryAfter.Date - DateTimeOffset.UtcNow);
                        if (delay.HasValue && (delay.Value > TimeSpan.Zero))
                        {
                            return ValueTask.FromResult<TimeSpan?>(delay.Value);
                        }
                    }
                    
                    // Fallback to exponential backoff: 2^attemptNumber seconds
                    var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, args.AttemptNumber));
                    return ValueTask.FromResult<TimeSpan?>(exponentialDelay);
                };

                // Ensure we retry on rate limit (429) and service unavailable (503) errors
                options.Retry.ShouldHandle = static args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests
                    or HttpStatusCode.ServiceUnavailable);

                // Configure circuit breaker to prevent overwhelming the API when it's having issues
                options.CircuitBreaker.FailureRatio = 0.5; // Break if 50% of requests fail
                options.CircuitBreaker.MinimumThroughput = 10; // Need at least 10 requests in sampling period
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30); // Monitor over 30 seconds
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30); // Stay broken for 30 seconds
                
                // Break on rate limit and server errors
                options.CircuitBreaker.ShouldHandle = static args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests
                    or HttpStatusCode.ServiceUnavailable
                    or HttpStatusCode.InternalServerError);

                // Set a reasonable overall timeout for API calls
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
                
                // Note: Actual rate limits depend on subscription level (10-1000+ RPM)
                // The retry-after header handling ensures we respect the API's rate limit guidance
            });
        }
    }
}
