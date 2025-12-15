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
    /// <param name="addPadding">When true, pads out responses to ensure all results contain a random number of records between 800 and 1,000 for enhanced privacy.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>A <see cref="PwnedPassword"/> object containing the password evaluation results.</returns>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="plainTextPassword"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<PwnedPassword> GetPwnedPasswordAsync(
        string plainTextPassword,
        bool addPadding = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="PwnedPassword"/> representing the evaluation of
    /// all hashed passwords in the HIBP range data using NTLM hash format.
    /// </summary>
    /// <param name="plainTextPassword">The plain text password to evaluate.</param>
    /// <param name="addPadding">When true, pads out responses to ensure all results contain a random number of records between 800 and 1,000 for enhanced privacy.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>A <see cref="PwnedPassword"/> object containing the password evaluation results using NTLM hashes.</returns>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="plainTextPassword"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<PwnedPassword> GetPwnedPasswordWithNtlmAsync(
        string plainTextPassword,
        bool addPadding = false,
        CancellationToken cancellationToken = default);
}
