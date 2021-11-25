// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary></summary>
public record PwnedPassword(
    string? PlainTextPassword,
    bool? IsPwned = default,
    long PwnedCount = -1,
    string? HashedPassword = default)
{
    internal bool IsInvalid() =>
        PlainTextPassword is null or { Length: 0 };
}
