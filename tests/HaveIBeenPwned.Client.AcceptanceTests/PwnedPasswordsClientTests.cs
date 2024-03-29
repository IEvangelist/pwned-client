// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

[Trait("Category", "AcceptanceTests")]
public sealed class PwnedPasswordsClientTests
{
    private readonly IPwnedPasswordsClient _passwordsClient = new PwnedClient(
            Environment.GetEnvironmentVariable("HibpOptions__ApiKey")!);

    [Fact]
    public async Task GetPwnedPasswordAsyncReturnsIsPwnedWithKnownPwnedPassword()
    {
        var pwnedPassword =
            await _passwordsClient.GetPwnedPasswordAsync("password");

        Assert.NotNull(pwnedPassword);
        Assert.True(pwnedPassword.IsPwned);
        Assert.True(pwnedPassword.PwnedCount > 9_659_000);
    }

    [Fact]
    public async Task IsPwnedPasswordAsyncRetrunsTrueForKnownPwnedPassword()
    {
        var (isPwned, count) =
            await _passwordsClient.IsPasswordPwnedAsync("p@ssw0rd");

        Assert.True(isPwned);
        Assert.True(count > 74_400);
    }
}