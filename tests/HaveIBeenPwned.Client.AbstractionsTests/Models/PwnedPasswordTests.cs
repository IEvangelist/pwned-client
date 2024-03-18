// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Abstractions;
using Xunit;

namespace HaveIBeenPwned.Client.AbstractionsTests.Models;

public class PwnedPasswordTests
{
    [
        Theory,
        InlineData(null, true),
        InlineData("", true),
        InlineData("   ", false),
        InlineData("someValue", false)
    ]
    public void PwnedPasswordReturnsCorrectValidityStateWhenConstructed(
        string? value, bool expected)
    {
        PwnedPassword pwnedPassword = new() { PlainTextPassword = value };
        Assert.Equal(expected, pwnedPassword.IsInvalid());
    }
}
