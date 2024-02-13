// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

[Trait("Category", "AcceptanceTests")]
public class PwnedClientTests
{
    private readonly IPwnedClient _pwnedClient;

    public PwnedClientTests() =>
        _pwnedClient = new PwnedClient(
            Environment.GetEnvironmentVariable("HibpOptions__ApiKey")!);

    [Fact]
    public async Task GetSubscriptionStatusAsyncReturnsCorrectStatus()
    {
        var status =
            await _pwnedClient.GetSubscriptionStatusAsync();

        Assert.NotNull(status);

        var actual = status.GetSubscriptionLevel();
        Assert.Equal(HibpSubscriptionLevel.One, actual);
    }
}
