// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

public sealed class PwnedPastesClientTests
{
    private readonly IPwnedPastesClient _pastesClient;

    public PwnedPastesClientTests() =>
        _pastesClient = new PwnedClient(
            Environment.GetEnvironmentVariable("HibpOptions__ApiKey")!);

    [Fact]
    public async Task GetPastesAsyncReturnsSinglePaste()
    {
        var pastes =
            await _pastesClient.GetPastesAsync(TestAccounts.PasteSensitiveBreach);

        Assert.NotNull(pastes);
        Assert.Single(pastes);
    }

    [Fact]
    public async Task GetPastesAsyncReturnsNoPastes()
    {
        var pastes =
            await _pastesClient.GetPastesAsync(TestAccounts.SensitiveAndOtherBreaches);

        Assert.NotNull(pastes);
        Assert.Empty(pastes);
    }
}
