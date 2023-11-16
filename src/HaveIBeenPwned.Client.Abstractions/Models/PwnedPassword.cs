// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// An object used to represent the plain-text password, and corresponding
/// hashed password. As well as whether the password is considered "pwned",
/// and if so, how many times.
/// </summary>
public sealed record class PwnedPassword
{
    /// <summary>
    /// The plain text password used for the lookup.
    /// </summary>
    public string? PlainTextPassword { get; set; }

    /// <summary>
    /// Whether or not the current <see cref="PwnedPassword"/>
    /// instance is considered to be "pwned".
    /// </summary>
    public bool? IsPwned { get; set; }

    /// <summary>
    /// When <see cref="IsPwned"/> is <c>true</c>, this will be a non-zero number.
    /// It represents the number of times the given <see cref="PlainTextPassword"/>
    /// has been found in the "have i been pwned" passwords database.
    /// </summary>
    public long PwnedCount { get; set; }

    /// <summary>
    ///  The hashed representation of the given <see cref="PlainTextPassword"/>.
    /// </summary>
    public string? HashedPassword { get; set; }

    internal bool IsInvalid() =>
        PlainTextPassword is null or { Length: 0 };
}
