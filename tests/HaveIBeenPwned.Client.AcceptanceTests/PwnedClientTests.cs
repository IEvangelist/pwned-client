// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

[Trait("Category", "AcceptanceTests")]
public class PwnedClientTests
{
    private const string TestApiKey = "00000000000000000000000000000000";

    private static readonly string s_apiKey =
        Environment.GetEnvironmentVariable("HibpOptions__ApiKey")
        ?? TestApiKey;

    private readonly IPwnedClient _pwnedClient = new PwnedClient(s_apiKey);

    [Fact(Skip = "This test requires a real API key with an active subscription.")]
    public async Task GetSubscriptionStatusAsyncReturnsCorrectStatus()
    {
        var status =
            await _pwnedClient.GetSubscriptionStatusAsync();

        Assert.NotNull(status);

        var actual = status.GetSubscriptionLevel();
        Assert.Equal(HibpSubscriptionLevel.One, actual);
    }
}
