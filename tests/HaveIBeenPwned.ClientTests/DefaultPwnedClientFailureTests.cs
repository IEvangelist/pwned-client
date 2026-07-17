// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using HaveIBeenPwned.Client.Factories;
using HaveIBeenPwned.Client.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

public sealed class DefaultPwnedClientFailureTests
{
    [Fact]
    public async Task DocumentedNotFoundResponsesReturnNoResult()
    {
        using var factory = new TestHttpClientFactory(_ =>
            new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = CreateClient(factory);

        var breaches = await client.GetBreachHeadersForAccountAsync("nobody@example.com");
        var pastes = await client.GetPastesAsync("nobody@example.com");
        var stealerLogs = await client.GetStealerLogsByEmailAsync("nobody@example.com");
        var domain = await client.GetBreachedDomainAsync("example.com");

        Assert.Empty(breaches);
        Assert.Empty(pastes);
        Assert.Null(stealerLogs);
        Assert.Null(domain);
    }

    [Fact]
    public async Task NonSuccessResponseThrowsTypedExceptionWithBody()
    {
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Text(
                "Access denied due to invalid hibp-api-key.",
                HttpStatusCode.Unauthorized));
        var client = CreateClient(factory);

        var exception = await Assert.ThrowsAsync<PwnedApiException>(
            () => client.GetSubscriptionStatusAsync());

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(
            "Access denied due to invalid hibp-api-key.",
            exception.ResponseContent);
        Assert.Null(exception.RetryAfter);
    }

    [
        Theory,
        InlineData(HttpStatusCode.BadRequest),
        InlineData(HttpStatusCode.Forbidden),
        InlineData(HttpStatusCode.ServiceUnavailable)
    ]
    public async Task OtherDocumentedFailuresThrowTypedException(
        HttpStatusCode statusCode)
    {
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json(
                """{"message":"Request failed."}""",
                statusCode));
        var client = CreateClient(factory);

        var exception = await Assert.ThrowsAsync<PwnedApiException>(
            () => client.GetSubscriptionStatusAsync());

        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Contains("Request failed", exception.ResponseContent);
    }

    [Fact]
    public async Task RateLimitResponsePreservesRetryAfter()
    {
        using var factory = new TestHttpClientFactory(_ =>
        {
            var response = TestHttpClientFactory.Json(
                """{"statusCode":429,"message":"Rate limit exceeded."}""",
                HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter =
                new System.Net.Http.Headers.RetryConditionHeaderValue(
                    TimeSpan.FromSeconds(3));
            return response;
        });
        var client = CreateClient(factory);

        var exception = await Assert.ThrowsAsync<PwnedApiException>(
            () => client.GetBreachHeadersForAccountAsync("user@example.com"));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(TimeSpan.FromSeconds(3), exception.RetryAfter);
        Assert.Contains("Rate limit exceeded", exception.ResponseContent);
    }

    [Fact]
    public async Task CancellationIsNeverConvertedToAnEmptyResult()
    {
        using var factory = new TestHttpClientFactory(
            async (_, cancellationToken) =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                return TestHttpClientFactory.Json("[]");
            });
        var client = CreateClient(factory);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetDataClassesAsync(cancellationTokenSource.Token));
    }

    [Fact]
    public async Task TransportAndSerializationFailuresRemainExplicit()
    {
        using var transportFactory = new TestHttpClientFactory(
            (_, _) => throw new HttpRequestException("Network unavailable."));
        var transportClient = CreateClient(transportFactory);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => transportClient.GetDataClassesAsync());

        using var serializationFactory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json("{not-json"));
        var serializationClient = CreateClient(serializationFactory);

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(
            () => serializationClient.GetDataClassesAsync());
    }

    [Fact]
    public void NonDiClientsKeepCredentialsIsolatedAndPasswordsUnauthenticated()
    {
        var firstFactory = (IHttpClientFactory)InternalHttpClientFactory.Create("first-key");
        var secondFactory = (IHttpClientFactory)InternalHttpClientFactory.Create("second-key");
        var anonymousFactory = (IHttpClientFactory)InternalHttpClientFactory.Create(null);

        var firstHibp = firstFactory.CreateClient(HttpClientNames.HibpClient);
        var secondHibp = secondFactory.CreateClient(HttpClientNames.HibpClient);
        var anonymousHibp = anonymousFactory.CreateClient(HttpClientNames.HibpClient);
        var passwords = firstFactory.CreateClient(HttpClientNames.PasswordsClient);

        Assert.Equal(
            ["first-key"],
            firstHibp.DefaultRequestHeaders.GetValues(HttpHeaderNames.HibpApiKey));
        Assert.Equal(
            ["second-key"],
            secondHibp.DefaultRequestHeaders.GetValues(HttpHeaderNames.HibpApiKey));
        Assert.False(
            anonymousHibp.DefaultRequestHeaders.Contains(HttpHeaderNames.HibpApiKey));
        Assert.False(
            passwords.DefaultRequestHeaders.Contains(HttpHeaderNames.HibpApiKey));
        Assert.NotEmpty(passwords.DefaultRequestHeaders.UserAgent);
    }

    [
        Theory,
        InlineData(null),
        InlineData(""),
        InlineData("   ")
    ]
    public async Task AccountAndDomainArgumentsAreValidated(string? value)
    {
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json("[]"));
        var client = CreateClient(factory);

        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetBreachHeadersForAccountAsync(value!));
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GenerateDomainVerificationDnsTokenAsync(value!));
    }

    private static IPwnedClient CreateClient(TestHttpClientFactory factory) =>
        new DefaultPwnedClient(
            factory,
            NullLogger<DefaultPwnedClient>.Instance);
}
