// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient
{
    private async Task<T?> GetJsonAsync<T>(
        string clientName,
        string requestUri,
        JsonTypeInfo<T> jsonTypeInfo,
        bool notFoundIsEmpty,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await SendAsync(
                clientName,
                request,
                notFoundIsEmpty,
                cancellationToken)
            .ConfigureAwait(false);

        if (response is null)
        {
            return default;
        }

        return await response.Content
            .ReadFromJsonAsync(jsonTypeInfo, cancellationToken)
            .ConfigureAwait(false);
    }

    private async IAsyncEnumerable<T?> GetJsonArrayAsync<T>(
        string clientName,
        string requestUri,
        JsonTypeInfo<T> jsonTypeInfo,
        bool notFoundIsEmpty,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await SendAsync(
                clientName,
                request,
                notFoundIsEmpty,
                cancellationToken)
            .ConfigureAwait(false);

        if (response is null)
        {
            yield break;
        }

        await foreach (var item in response.Content
            .ReadFromJsonAsAsyncEnumerable(jsonTypeInfo, cancellationToken)
            .ConfigureAwait(false))
        {
            yield return item;
        }
    }

    private async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(
        string requestUri,
        TRequest content,
        JsonTypeInfo<TRequest> requestTypeInfo,
        JsonTypeInfo<TResponse> responseTypeInfo,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(content, requestTypeInfo)
        };
        using var response = await SendAsync(
                HibpClient,
                request,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false);

        return await response!.Content
            .ReadFromJsonAsync(responseTypeInfo, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task PostJsonAsync<TRequest>(
        string requestUri,
        TRequest content,
        JsonTypeInfo<TRequest> requestTypeInfo,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(content, requestTypeInfo)
        };
        using var response = await SendAsync(
                HibpClient,
                request,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage?> SendAsync(
        string clientName,
        HttpRequestMessage request,
        bool notFoundIsEmpty,
        CancellationToken cancellationToken)
    {
        var telemetry = GetTelemetryOperation(clientName, request);
        using var activity = PwnedClientTelemetry.ActivitySource.StartActivity(
            $"hibp.{telemetry.Operation}",
            ActivityKind.Client);
        activity?.SetTag("hibp.operation", telemetry.Operation);
        activity?.SetTag("hibp.api", telemetry.ApiSurface);
        activity?.SetTag("http.request.method", request.Method.Method);
        activity?.SetTag("server.address", telemetry.ServerAddress);
        activity?.SetTag("url.template", telemetry.UrlTemplate);

        var startedAt = Stopwatch.GetTimestamp();
        var outcome = "success";
        int? statusCode = null;
        string? errorType = null;
        var failureRecorded = false;

        try
        {
            var client = httpClientFactory.CreateClient(clientName);
            var response = await client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken)
                .ConfigureAwait(false);
            statusCode = (int)response.StatusCode;
            activity?.SetTag("http.response.status_code", statusCode);

            if (notFoundIsEmpty && response.StatusCode is HttpStatusCode.NotFound)
            {
                outcome = "not_found";
                activity?.SetStatus(ActivityStatusCode.Ok);
                response.Dispose();
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
                return response;
            }

            outcome = "error";
            RecordFailure(telemetry, request.Method, statusCode);
            failureRecorded = true;

            using (response)
            {
                var responseContent = await response.Content
                    .ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);
                var retryAfter = GetRetryAfter(response);
                var message =
                    $"The HIBP API returned {(int)response.StatusCode} ({response.ReasonPhrase}).";

                logger.LogError(
                    "HIBP operation {Operation} failed with status code {StatusCode}.",
                    telemetry.Operation,
                    statusCode);

                throw new PwnedApiException(
                    message,
                    response.StatusCode,
                    string.IsNullOrWhiteSpace(responseContent) ? null : responseContent,
                    retryAfter);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            outcome = "canceled";
            errorType = typeof(OperationCanceledException).FullName;
            throw;
        }
        catch (Exception exception)
        {
            outcome = "error";
            errorType = exception.GetType().FullName;
            activity?.SetStatus(ActivityStatusCode.Error);
            if (!failureRecorded)
            {
                RecordFailure(telemetry, request.Method, statusCode);
            }

            throw;
        }
        finally
        {
            activity?.SetTag("hibp.outcome", outcome);
            if (errorType is not null)
            {
                activity?.SetTag("error.type", errorType);
            }

            var tags = CreateMetricTags(
                telemetry,
                request.Method,
                outcome,
                statusCode);
            PwnedClientTelemetry.RequestCount.Add(1, tags);
            PwnedClientTelemetry.RequestDuration.Record(
                Stopwatch.GetElapsedTime(startedAt).TotalSeconds,
                tags);
        }
    }

    private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter is not { } retryAfter)
        {
            return null;
        }

        if (retryAfter.Delta is { } delta)
        {
            return delta;
        }

        if (retryAfter.Date is not { } date)
        {
            return null;
        }

        var delay = date - DateTimeOffset.UtcNow;
        return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
    }

    private static string EscapePathSegment(string value) =>
        Uri.EscapeDataString(value.Trim());

    private static string BuildRequestUri(
        string path,
        params (string Name, string? Value)[] queryParameters)
    {
        var query = queryParameters
            .Where(static parameter => parameter.Value is not null)
            .Select(static parameter =>
                $"{Uri.EscapeDataString(parameter.Name)}={Uri.EscapeDataString(parameter.Value!)}");

        var queryString = string.Join("&", query);
        return queryString.Length is 0 ? path : $"{path}?{queryString}";
    }

    private static void RecordFailure(
        TelemetryOperation telemetry,
        HttpMethod method,
        int? statusCode)
    {
        var tags = CreateMetricTags(
            telemetry,
            method,
            outcome: "error",
            statusCode);
        PwnedClientTelemetry.FailureCount.Add(1, tags);

        if (statusCode is (int)HttpStatusCode.TooManyRequests)
        {
            PwnedClientTelemetry.RateLimitCount.Add(1, tags);
        }
    }

    private static TagList CreateMetricTags(
        TelemetryOperation telemetry,
        HttpMethod method,
        string outcome,
        int? statusCode)
    {
        TagList tags = default;
        tags.Add("hibp.operation", telemetry.Operation);
        tags.Add("hibp.api", telemetry.ApiSurface);
        tags.Add("http.request.method", method.Method);
        tags.Add("hibp.outcome", outcome);
        if (statusCode is not null)
        {
            tags.Add("http.response.status_code", statusCode.Value);
        }

        return tags;
    }

    private static TelemetryOperation GetTelemetryOperation(
        string clientName,
        HttpRequestMessage request)
    {
        if (clientName == PasswordsClient)
        {
            return new(
                "passwords.range",
                "pwned-passwords",
                "api.pwnedpasswords.com",
                "/range/{hash-prefix}");
        }

        var path = request.RequestUri?.OriginalString
            .Split('?', 2)[0]
            .TrimStart('/') ?? "";

        if (path.StartsWith("breachedaccount/range/", StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "breaches.account.range",
                "/breachedaccount/range/{hash-prefix}");
        }

        if (path.StartsWith("breachedaccount/", StringComparison.OrdinalIgnoreCase))
        {
            return V3("breaches.account", "/breachedaccount/{account}");
        }

        if (path.StartsWith("breach/", StringComparison.OrdinalIgnoreCase))
        {
            return V3("breaches.get", "/breach/{name}");
        }

        if (path.Equals("breaches", StringComparison.OrdinalIgnoreCase))
        {
            return V3("breaches.list", "/breaches");
        }

        if (path.Equals("latestbreach", StringComparison.OrdinalIgnoreCase))
        {
            return V3("breaches.latest", "/latestbreach");
        }

        if (path.Equals("dataclasses", StringComparison.OrdinalIgnoreCase))
        {
            return V3("breaches.data_classes", "/dataclasses");
        }

        if (path.Equals(
            "domainverification/generatednstoken",
            StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "domains.generate_dns_token",
                "/domainverification/generatednstoken");
        }

        if (path.Equals(
            "domainverification/verifydnstoken",
            StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "domains.verify_dns_token",
                "/domainverification/verifydnstoken");
        }

        if (path.Equals(
            "domainverification/sendemail",
            StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "domains.send_verification_email",
                "/domainverification/sendemail");
        }

        if (path.StartsWith("breacheddomain/", StringComparison.OrdinalIgnoreCase))
        {
            return V3("domains.breaches", "/breacheddomain/{domain}");
        }

        if (path.Equals("subscribeddomains", StringComparison.OrdinalIgnoreCase))
        {
            return V3("domains.subscribed", "/subscribeddomains");
        }

        if (path.StartsWith("stealerlogsbyemail/", StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "stealer_logs.email",
                "/stealerlogsbyemail/{email-address}");
        }

        if (path.StartsWith(
            "stealerlogsbywebsitedomain/",
            StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "stealer_logs.website_domain",
                "/stealerlogsbywebsitedomain/{domain}");
        }

        if (path.StartsWith(
            "stealerlogsbyemaildomain/",
            StringComparison.OrdinalIgnoreCase))
        {
            return V3(
                "stealer_logs.email_domain",
                "/stealerlogsbyemaildomain/{domain}");
        }

        if (path.StartsWith("pasteaccount/", StringComparison.OrdinalIgnoreCase))
        {
            return V3("pastes.account", "/pasteaccount/{account}");
        }

        if (path.Equals("subscription/status", StringComparison.OrdinalIgnoreCase))
        {
            return V3("subscription.status", "/subscription/status");
        }

        return V3("unknown", "/unknown");
    }

    private static TelemetryOperation V3(
        string operation,
        string urlTemplate) =>
        new(operation, "hibp-v3", "haveibeenpwned.com", urlTemplate);

    private readonly record struct TelemetryOperation(
        string Operation,
        string ApiSurface,
        string ServerAddress,
        string UrlTemplate);
}
