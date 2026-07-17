// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Text.Json;
using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

public sealed class DefaultPwnedClientContractTests
{
    [Fact]
    public async Task BreachCatalogueOperationsUseDocumentedRoutes()
    {
        using var factory = new TestHttpClientFactory(request =>
            request.Uri.AbsolutePath switch
            {
                "/api/v3/breaches" => TestHttpClientFactory.Json(
                    """[{"name":"Adobe","title":"Adobe"}]"""),
                "/api/v3/breach/Adobe%2F2026" => TestHttpClientFactory.Json(
                    """{"name":"Adobe","title":"Adobe"}"""),
                "/api/v3/latestbreach" => TestHttpClientFactory.Json(
                    """{"name":"Adobe","title":"Adobe"}"""),
                "/api/v3/dataclasses" => TestHttpClientFactory.Json(
                    """["Email addresses","Passwords"]"""),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            });
        var client = CreateAggregateClient(factory);

        var breaches = await client.GetAllBreachDetailsAsync(
            "example.com/a b",
            isSpamList: true);
        var breach = await client.GetBreachAsync("Adobe/2026");
        var latest = await client.GetLatestBreachAsync();
        var dataClasses = await client.GetDataClassesAsync();

        Assert.Single(breaches);
        Assert.Equal("Adobe", breach?.Name);
        Assert.Equal("Adobe", latest?.Name);
        Assert.Equal(["Email addresses", "Passwords"], dataClasses);
        Assert.Equal(
            "/api/v3/breaches?domain=example.com%2Fa%20b&isSpamList=true",
            factory.Requests[0].Uri.PathAndQuery);
        Assert.Equal("/api/v3/breach/Adobe%2F2026", factory.Requests[1].Uri.PathAndQuery);
        Assert.Equal("/api/v3/latestbreach", factory.Requests[2].Uri.PathAndQuery);
        Assert.Equal("/api/v3/dataclasses", factory.Requests[3].Uri.PathAndQuery);
    }

    [Fact]
    public async Task DirectAccountSearchSupportsEveryDocumentedFilter()
    {
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json("""[{"name":"Adobe","title":"Adobe"}]"""));
        var client = CreateClient(factory);

        var headers = await client.GetBreachHeadersForAccountAsync(
            " user+tag@example.com ",
            includeUnverified: false,
            domain: "example.com/tenant");
        var details = await client.GetBreachesForAccountAsync(
            "user+tag@example.com",
            includeUnverified: false,
            domain: "example.com/tenant");

        Assert.Single(headers);
        Assert.Single(details);
        Assert.Equal(
            "/api/v3/breachedaccount/user%2Btag%40example.com" +
            "?truncateResponse=true&includeUnverified=false&domain=example.com%2Ftenant",
            factory.Requests[0].Uri.PathAndQuery);
        Assert.Equal(
            "/api/v3/breachedaccount/user%2Btag%40example.com" +
            "?truncateResponse=false&includeUnverified=false&domain=example.com%2Ftenant",
            factory.Requests[1].Uri.PathAndQuery);
    }

    [Fact]
    public async Task AccountKAnonymitySearchSendsOnlyPrefixAndReturnsLocalMatch()
    {
        const string account = "multiple-breaches@hibp-integration-tests.com";
        const string matchingSuffix = "C6C0AADE0C085843D66E4944E108C4A4CD";
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json(
                $$"""
                [
                  {"hashSuffix":"0000000000000000000000000000000000","websites":["Discarded"]},
                  {"hashSuffix":"{{matchingSuffix}}","websites":["Adobe","Gawker","Stratfor"]}
                ]
                """));
        var client = CreateClient(factory);

        var breaches = await client.GetBreachHeadersForAccountUsingKAnonymityAsync(
            $" {account.ToUpperInvariant()} ");

        Assert.Equal(["Adobe", "Gawker", "Stratfor"], breaches.Select(static b => b.Name));
        Assert.Equal(
            "/api/v3/breachedaccount/range/6b5917",
            factory.Requests.Single().Uri.PathAndQuery);
        Assert.DoesNotContain(
            breaches,
            static breach => breach.Name == "Discarded");
    }

    [Fact]
    public async Task DomainOperationsUseDocumentedMethodsBodiesAndRoutes()
    {
        using var factory = new TestHttpClientFactory(request =>
            request.Uri.AbsolutePath switch
            {
                "/api/v3/domainverification/generatednstoken" =>
                    TestHttpClientFactory.Json(
                        """{"txtRecordValue":"hibp-verify=dweb_test"}"""),
                "/api/v3/domainverification/verifydnstoken" or
                "/api/v3/domainverification/sendemail" =>
                    new HttpResponseMessage(HttpStatusCode.OK),
                "/api/v3/breacheddomain/example.com%2Ftenant" =>
                    TestHttpClientFactory.Json("""{"admin":["Adobe"]}"""),
                "/api/v3/subscribeddomains" =>
                    TestHttpClientFactory.Json(
                        """[{"domainName":"example.com","pwnCount":12}]"""),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            });
        var client = CreateAggregateClient(factory);

        var token = await client.GenerateDomainVerificationDnsTokenAsync(" example.com ");
        await client.VerifyDomainViaDnsAsync("example.com");
        await client.SendDomainVerificationEmailAsync(
            "example.com",
            DomainVerificationEmailAlias.Security);
        var domainBreaches = await client.GetBreachedDomainAsync("example.com/tenant");
        var subscribedDomains = await client.GetSubscribedDomainsAsync();

        Assert.Equal("hibp-verify=dweb_test", token.TxtRecordValue);
        Assert.NotNull(domainBreaches);
        Assert.Equal(["Adobe"], domainBreaches["admin"]);
        Assert.Equal("example.com", Assert.Single(subscribedDomains).DomainName);
        Assert.All(
            factory.Requests.Take(3),
            static request => Assert.Equal(HttpMethod.Post, request.Method));
        AssertJson(
            """{"DomainName":"example.com"}""",
            factory.Requests[0].Content);
        AssertJson(
            """{"DomainName":"example.com"}""",
            factory.Requests[1].Content);
        AssertJson(
            """{"DomainName":"example.com","EmailAlias":"security"}""",
            factory.Requests[2].Content);
        Assert.Equal(
            "/api/v3/breacheddomain/example.com%2Ftenant",
            factory.Requests[3].Uri.PathAndQuery);
        Assert.Equal(
            "/api/v3/subscribeddomains",
            factory.Requests[4].Uri.PathAndQuery);
    }

    [
        Theory,
        InlineData(DomainVerificationEmailAlias.Admin, "admin"),
        InlineData(DomainVerificationEmailAlias.Hostmaster, "hostmaster"),
        InlineData(DomainVerificationEmailAlias.Info, "info"),
        InlineData(DomainVerificationEmailAlias.Security, "security"),
        InlineData(DomainVerificationEmailAlias.Webmaster, "webmaster")
    ]
    public async Task DomainVerificationSupportsEveryDocumentedEmailAlias(
        DomainVerificationEmailAlias emailAlias,
        string expected)
    {
        using var factory = new TestHttpClientFactory(_ =>
            new HttpResponseMessage(HttpStatusCode.OK));
        var client = CreateAggregateClient(factory);

        await client.SendDomainVerificationEmailAsync("example.com", emailAlias);

        using var content = JsonDocument.Parse(factory.Requests.Single().Content!);
        Assert.Equal(
            expected,
            content.RootElement.GetProperty("EmailAlias").GetString());
    }

    [Fact]
    public async Task StealerPasteAndSubscriptionOperationsMatchCurrentModels()
    {
        using var factory = new TestHttpClientFactory(request =>
            request.Uri.AbsolutePath switch
            {
                "/api/v3/stealerlogsbyemail/user%2Btag%40example.com" =>
                    TestHttpClientFactory.Json("""["netflix.com"]"""),
                "/api/v3/stealerlogsbywebsitedomain/example.com%2Flogin" =>
                    TestHttpClientFactory.Json("""["user@example.com"]"""),
                "/api/v3/stealerlogsbyemaildomain/example.com" =>
                    TestHttpClientFactory.Json("""{"user":["netflix.com"]}"""),
                "/api/v3/pasteaccount/user%2Btag%40example.com" =>
                    TestHttpClientFactory.Json(
                        """[{"source":"Pastebin","id":"123","emailCount":1}]"""),
                "/api/v3/subscription/status" =>
                    TestHttpClientFactory.Json(
                        """
                        {
                          "subscriptionName":"Pro 2",
                          "description":"Current plan",
                          "subscribedUntil":"2027-01-01T00:00:00Z",
                          "rpm":2000,
                          "domainSearchMaxBreachedAccounts":500,
                          "maxBreachedDomains":100,
                          "includesStealerLogs":true,
                          "includesBulkDomainAdd":true,
                          "includesAutoSubdomainVerification":true,
                          "includesCustomerDomains":true,
                          "includesKAnon":true
                        }
                        """),
                _ => new HttpResponseMessage(HttpStatusCode.NotFound)
            });
        var aggregateClient = CreateAggregateClient(factory);

        var emailDomains = await aggregateClient.GetStealerLogsByEmailAsync(
            "user+tag@example.com");
        var websiteEmails = await aggregateClient.GetStealerLogsByWebsiteDomainAsync(
            "example.com/login");
        var aliases = await aggregateClient.GetStealerLogsByEmailDomainAsync("example.com");
        var pastes = await aggregateClient.GetPastesAsync("user+tag@example.com");
        var subscription = await aggregateClient.GetSubscriptionStatusAsync();

        Assert.NotNull(emailDomains);
        Assert.NotNull(websiteEmails);
        Assert.NotNull(aliases);
        Assert.Equal(["netflix.com"], emailDomains);
        Assert.Equal(["user@example.com"], websiteEmails);
        Assert.Equal(["netflix.com"], aliases["user"]);
        Assert.Equal("Pastebin", Assert.Single(pastes).Source);
        Assert.NotNull(subscription);
        Assert.Equal(100, subscription.MaxBreachedDomains);
        Assert.True(subscription.IncludesStealerLogs);
        Assert.True(subscription.IncludesBulkDomainAdd);
        Assert.True(subscription.IncludesAutoSubdomainVerification);
        Assert.True(subscription.IncludesCustomerDomains);
        Assert.True(subscription.IncludesKAnon);
    }

    [Fact]
    public async Task PasswordRangeSupportsSha1NtlmAndPadding()
    {
        using var factory = new TestHttpClientFactory(request =>
            request.Uri.Query == "?mode=ntlm"
                ? TestHttpClientFactory.Text(
                    "7EAEE8FB117AD06BDD830B7586C:42\r\n")
                : TestHttpClientFactory.Text(
                    "00000000000000000000000000000000000:0\r\n" +
                    "1E4C9B93F3F0682250B6CF8331B7EE68FD8:3303003\r\n"));
        var client = (IPwnedPasswordsClient)CreateAggregateClient(factory);

        var sha1 = await client.GetPwnedPasswordAsync("password", addPadding: true);
        var ntlm = await client.GetPwnedPasswordWithNtlmAsync("password");

        Assert.True(sha1.IsPwned);
        Assert.Equal(3_303_003, sha1.PwnedCount);
        Assert.True(ntlm.IsPwned);
        Assert.Equal(42, ntlm.PwnedCount);
        Assert.Equal("/range/5baa6", factory.Requests[0].Uri.PathAndQuery);
        Assert.Equal(["true"], factory.Requests[0].Headers["Add-Padding"]);
        Assert.Equal("/range/8846f?mode=ntlm", factory.Requests[1].Uri.PathAndQuery);
        Assert.DoesNotContain("Add-Padding", factory.Requests[1].Headers);
    }

    [Fact]
    public async Task AsyncEnumerableOperationsStreamJsonArrays()
    {
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json("""[{"name":"Adobe"},{"name":"Gawker"}]"""));
        var client = CreateClient(factory);
        List<string> names = [];

        await foreach (var breach in client.GetBreachHeadersForAccountAsAsyncEnumerable(
            "user@example.com",
            includeUnverified: true))
        {
            names.Add(breach!.Name);
        }

        Assert.Equal(["Adobe", "Gawker"], names);
    }

    private static IPwnedBreachesClient CreateClient(TestHttpClientFactory factory) =>
        CreateAggregateClient(factory);

    private static IPwnedClient CreateAggregateClient(TestHttpClientFactory factory) =>
        new DefaultPwnedClient(
            factory,
            NullLogger<DefaultPwnedClient>.Instance);

    private static void AssertJson(string expected, string? actual)
    {
        Assert.NotNull(actual);
        using var expectedDocument = JsonDocument.Parse(expected);
        using var actualDocument = JsonDocument.Parse(actual);
        Assert.Equal(
            expectedDocument.RootElement.ToString(),
            actualDocument.RootElement.ToString());
    }
}
