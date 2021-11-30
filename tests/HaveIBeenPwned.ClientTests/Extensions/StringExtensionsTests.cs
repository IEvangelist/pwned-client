// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using HaveIBeenPwned.Client.Extensions;

namespace HaveIBeenPwned.ClientTests.Extensions;

public class StringExtensionsTests
{
    [
        Theory,
        InlineData(null, null),
        InlineData("", "")
    ]
    public void ToSha1Hash_Returns_UselessValues_WhenGiven_UselessValues(
        string value, string expected)
    {
        var actual = value.ToSha1Hash();
        Assert.Equal(expected, actual);
    }

    [
        Theory,
        InlineData("Does this even work?", "5ec3e89686893f2ba76bdf1bcf1070568823e400"),
        InlineData("       ", "bab791e157462ccf081f4d7b85ca026b3a1940cd"),
        InlineData("f@k3PA55w0d!", "a841ab792c6b438c71a97e05e6197ceba54f8e96")
    ]
    public void ToSha1Hash_Returns_ExpectedHash_WhenGiven_ValidValues(
        string value, string expected)
    {
        var actual = value.ToSha1Hash();
        Assert.Equal(expected, actual);
    }
}
