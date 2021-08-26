// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Models
{
    public record PwnedPassword(
        string? PlainTextPassword,
        bool IsPwned = false,
        int PwnedCount = -1,
        string? HashedPassword = default)
    {
        internal bool IsInvalid() =>
            PlainTextPassword is null or { Length: 0 };
    }
}
