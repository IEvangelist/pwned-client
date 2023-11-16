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
            shouldRedactHeaderValue: static headerName =>
            {
                return s_redactedHttpHeaders.Any(
                    predicate: name =>
                    {
                        return string.Equals(
                            name, headerName, StringComparison.OrdinalIgnoreCase);
                    });
            });

        if (configureResilienceOptions is not null)
        {
            _ = builder.AddStandardResilienceHandler(configureResilienceOptions);
        }
        else
        {
            builder.AddStandardResilienceHandler(static options =>
            {
                // TODO: Add the retry policy for the HIBP API based on the rate limiting:
                //       https://haveibeenpwned.com/API/v3#RateLimiting
            });
        }
    }
}
