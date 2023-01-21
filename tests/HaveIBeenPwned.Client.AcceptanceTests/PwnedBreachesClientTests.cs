// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

[Trait("Category", "AcceptanceTests")]
public sealed class PwnedBreachesClientTests
{
    private readonly IPwnedBreachesClient _breachesClient;

    public PwnedBreachesClientTests() =>
        _breachesClient = new PwnedClient(
            Environment.GetEnvironmentVariable("HibpOptions__ApiKey")!);

    [
        Theory(Skip = "These documented test accounts inconsistently return breaches."),
        InlineData(TestAccounts.UnverifiedBreach, 0 /* 1 unverified */),
        InlineData(TestAccounts.NotActiveBreach, 0),
        InlineData(TestAccounts.AccountExists, 1),
        InlineData(TestAccounts.NotActiveAndActiveBreach, 1 /* 1 inactive */),
        InlineData(TestAccounts.MultipleBreaches, 3)
    ]
    public async Task GetBreachesForAccountAsyncReturnsExpectedBreachDetails(
        string email, int expectedCount)
    {
        var breaches =
            await _breachesClient.GetBreachesForAccountAsync(email);

        Assert.NotNull(breaches);
        Assert.Equal(expectedCount, breaches.Length);
    }

    [Fact]
    public async Task GetBreachHeaderAsyncReturnsValues()
    {
        var breaches = await _breachesClient.GetBreachesAsync();
        Assert.NotNull(breaches);
        Assert.NotEmpty(breaches);

        var randomBreach =
            breaches.ElementAt(Random.Shared.Next(breaches.Length - 1));

        var breach = await _breachesClient.GetBreachAsync(randomBreach.Name);
        Assert.NotNull(breach);
    }
}
