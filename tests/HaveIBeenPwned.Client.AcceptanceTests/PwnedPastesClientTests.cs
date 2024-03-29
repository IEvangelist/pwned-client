﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Abstractions;

namespace HaveIBeenPwned.Client.AcceptanceTests;

[Trait("Category", "AcceptanceTests")]
public sealed class PwnedPastesClientTests
{
    private readonly IPwnedPastesClient _pastesClient = new PwnedClient(
        Environment.GetEnvironmentVariable("HibpOptions__ApiKey")!);

    [Fact]
    public async Task GetPastesAsyncReturnsNoPastes()
    {
        var pastes =
            await _pastesClient.GetPastesAsync(TestAccounts.SensitiveAndOtherBreaches);

        Assert.NotNull(pastes);
        Assert.Empty(pastes);
    }

    [Fact(Skip = "These documented test accounts inconsistently return pastes.")]
    public async Task GetPastesAsAsyncEnumerableReturnsSinglePaste()
    {
        List<Pastes> pastes = [];

        await foreach (var paste in _pastesClient.GetPastesAsAsyncEnumerable(TestAccounts.PasteSensitiveBreach))
        {
            if (paste is null)
            {
                continue;
            }

            pastes.Add(paste);
        }

        Assert.NotNull(pastes);
        Assert.Single(pastes);
    }
}
