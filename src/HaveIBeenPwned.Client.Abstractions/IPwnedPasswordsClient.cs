// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// The "pwned" passwords client.
/// </summary>
public interface IPwnedPasswordsClient
{
    /// <summary>
    /// Gets the <see cref="PwnedPassword"/> representing the evaluation of
    /// all hashed passwords in the HIBP range data.
    /// </summary>
    /// <param name="plainTextPassword">The plain text password to evaluate.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="plainTextPassword"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<PwnedPassword> GetPwnedPasswordAsync(string plainTextPassword);
}
