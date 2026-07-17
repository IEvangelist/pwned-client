// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

public sealed class PwnedPasswordParsingTests
{
    [Fact]
    public void ParsePasswordRangeResponseHandlesCrLfAndMalformedRows()
    {
        const string hash = "A841AB792C6B438C71A97E05E6197CEBA54F8E96";
        const string response =
            "INVALID\r\n" +
            "B792C6B438C71A97E05E6197CEBA54F8E96:not-a-number\r\n" +
            "B792C6B438C71A97E05E6197CEBA54F8E96:77\r\n";

        var result = DefaultPwnedClient.ParsePasswordRangeResponseText(
            new PwnedPassword { PlainTextPassword = "f@k3PA55w0d!" },
            response,
            hash);

        Assert.True(result.IsPwned);
        Assert.Equal(77, result.PwnedCount);
        Assert.Equal(hash, result.HashedPassword);
    }

    [Fact]
    public void ParsePasswordRangeResponseReturnsNotPwnedWhenSuffixIsAbsent()
    {
        const string hash = "5EC3E89686893F2BA76BDF1BCF1070568823E400";

        var result = DefaultPwnedClient.ParsePasswordRangeResponseText(
            new PwnedPassword { PlainTextPassword = "Does this even work?" },
            "00000000000000000000000000000000000:12\r\n",
            hash);

        Assert.False(result.IsPwned);
        Assert.Equal(0, result.PwnedCount);
    }

    [Fact]
    public void ParsePasswordRangeResponseDiscardsPaddingCount()
    {
        const string hash = "A841AB792C6B438C71A97E05E6197CEBA54F8E96";

        var result = DefaultPwnedClient.ParsePasswordRangeResponseText(
            new PwnedPassword { PlainTextPassword = "f@k3PA55w0d!" },
            "B792C6B438C71A97E05E6197CEBA54F8E96:0\r\n",
            hash);

        Assert.False(result.IsPwned);
        Assert.Equal(0, result.PwnedCount);
    }
}
