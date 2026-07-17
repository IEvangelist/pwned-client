// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using HaveIBeenPwned.Client.Abstractions;
using HaveIBeenPwned.Client.Abstractions.Serialization;
using Xunit;

namespace HaveIBeenPwned.Client.AbstractionsTests.Models;

public sealed class ApiModelSerializationTests
{
    [Fact]
    public void BreachDetailsDeserializesEveryDocumentedField()
    {
        const string json =
            """
            {
              "name":"Adobe",
              "title":"Adobe",
              "domain":"adobe.com",
              "breachDate":"2013-10-04",
              "addedDate":"2013-12-04T00:00:00Z",
              "modifiedDate":"2022-05-15T23:52:49Z",
              "pwnCount":152445165,
              "description":"Description",
              "dataClasses":["Email addresses","Passwords"],
              "isVerified":true,
              "isFabricated":false,
              "isSensitive":false,
              "isRetired":false,
              "isSpamList":false,
              "isMalware":false,
              "isSubscriptionFree":true,
              "isStealerLog":true,
              "logoPath":"Adobe.png",
              "attribution":"Provider",
              "futureField":"ignored"
            }
            """;

        var breach = JsonSerializer.Deserialize(
            json,
            SourceGeneratorContext.Default.BreachDetails);

        Assert.NotNull(breach);
        Assert.Equal("Adobe", breach.Name);
        Assert.Equal("Adobe", breach.Title);
        Assert.Equal("adobe.com", breach.Domain);
        Assert.Equal(new DateTime(2013, 10, 4), breach.BreachDate);
        Assert.Equal(152_445_165, breach.PwnCount);
        Assert.Equal(["Email addresses", "Passwords"], breach.DataClasses);
        Assert.True(breach.IsVerified);
        Assert.False(breach.IsFabricated);
        Assert.False(breach.IsSensitive);
        Assert.False(breach.IsRetired);
        Assert.False(breach.IsSpamList);
        Assert.False(breach.IsMalware);
        Assert.True(breach.IsSubscriptionFree);
        Assert.True(breach.IsStealerLog);
        Assert.Equal("Adobe.png", breach.LogoPath);
        Assert.Equal("Provider", breach.Attribution);
    }

    [Fact]
    public void SubscriptionStatusDeserializesEveryCapability()
    {
        const string json =
            """
            {
              "subscriptionName":"Pro 2",
              "description":"Current plan",
              "subscribedUntil":"2027-01-01T00:00:00Z",
              "rpm":2000,
              "domainSearchMaxBreachedAccounts":500,
              "maxBreachedDomains":null,
              "includesStealerLogs":true,
              "includesBulkDomainAdd":true,
              "includesAutoSubdomainVerification":true,
              "includesCustomerDomains":true,
              "includesKAnon":true
            }
            """;

        var status = JsonSerializer.Deserialize(
            json,
            SourceGeneratorContext.Default.SubscriptionStatus);

        Assert.NotNull(status);
        Assert.Equal("Pro 2", status.SubscriptionName);
        Assert.Equal(2_000, status.Rpm);
        Assert.Equal(500, status.DomainSearchMaxBreachedAccounts);
        Assert.Null(status.MaxBreachedDomains);
        Assert.True(status.IncludesStealerLogs);
        Assert.True(status.IncludesBulkDomainAdd);
        Assert.True(status.IncludesAutoSubdomainVerification);
        Assert.True(status.IncludesCustomerDomains);
        Assert.True(status.IncludesKAnon);
    }

    [Fact]
    public void PasteAllowsOmittedNullableFields()
    {
        const string json =
            """{"source":"Pastebin","id":"123","emailCount":10}""";

        var paste = JsonSerializer.Deserialize(
            json,
            SourceGeneratorContext.Default.Pastes);

        Assert.NotNull(paste);
        Assert.Null(paste.Title);
        Assert.Null(paste.Date);
        Assert.Equal(10, paste.EmailCount);
    }

    [Fact]
    public void DomainModelsDeserializeDocumentedShapes()
    {
        var domains = JsonSerializer.Deserialize(
            """
            [{
              "domainName":"example.com",
              "pwnCount":10,
              "pwnCountExcludingSpamLists":8,
              "pwnCountExcludingSpamListsAtLastSubscriptionRenewal":7,
              "nextSubscriptionRenewal":"2027-01-01T00:00:00Z"
            }]
            """,
            SourceGeneratorContext.Default.SubscribedDomainArray);
        var token = JsonSerializer.Deserialize(
            """{"txtRecordValue":"hibp-verify=dweb_test"}""",
            SourceGeneratorContext.Default.DomainVerificationDnsToken);

        var domain = Assert.Single(domains!);
        Assert.Equal("example.com", domain.DomainName);
        Assert.Equal(10, domain.PwnCount);
        Assert.Equal(8, domain.PwnCountExcludingSpamLists);
        Assert.Equal(7, domain.PwnCountExcludingSpamListsAtLastSubscriptionRenewal);
        Assert.NotNull(domain.NextSubscriptionRenewal);
        Assert.Equal("hibp-verify=dweb_test", token?.TxtRecordValue);
    }
}
