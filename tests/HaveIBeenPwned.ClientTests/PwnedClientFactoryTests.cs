// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Factories;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

public sealed class PwnedClientFactoryTests
{
    [Fact]
    public void PwnedClientFactory_Throws_WhenApiIsNull() => Assert.Throws<ArgumentNullException>(() => PwnedClientFactory.FromApiKey(null!));

    [Fact]
    public void PwnedClientFactory_ReturnsValidClient_WithApiKey()
    {
        var client = PwnedClientFactory.FromApiKey("test");
        Assert.NotNull(client);
    }
}
