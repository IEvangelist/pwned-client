// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// The "pwned" stealer logs client for accessing stealer log data.
/// All stealer log APIs require a Pwned 5 subscription or higher.
/// </summary>
public interface IPwnedStealerLogsClient
{
    /// <summary>
    /// Gets all stealer log domains for a specific email address.
    /// The email address being searched for must be on a domain already added to the domain search dashboard.
    /// See <a href="https://haveibeenpwned.com/API/v3#StealerLogsByEmail"></a>
    /// </summary>
    /// <param name="emailAddress">The email address to search for in stealer logs.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of website domains where the email address was captured, sorted alphabetically, or null if not found.</returns>
    /// <remarks>
    /// This search is based on the full email address captured by an info stealer as the owner authenticated to a website.
    /// Requires a Pwned 5 subscription or higher.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="emailAddress"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<string[]?> GetStealerLogsByEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stealer log email addresses for a website domain.
    /// The domain being searched for must be already added to the domain search dashboard.
    /// See <a href="https://haveibeenpwned.com/API/v3#StealerLogsByWebsiteDomain"></a>
    /// </summary>
    /// <param name="domain">The website domain to search for in stealer logs.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of email addresses sorted alphabetically, or null if not found.</returns>
    /// <remarks>
    /// This search is by the domain of the website URLs that appear in stealer logs.
    /// Typically performed by a website operator looking to identify which customers are likely subject to account takeover attacks.
    /// Requires a Pwned 5 subscription or higher.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="domain"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<string[]?> GetStealerLogsByWebsiteDomainAsync(
        string domain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stealer log email aliases for an email domain.
    /// The domain being searched for must be already added to the domain search dashboard.
    /// See <a href="https://haveibeenpwned.com/API/v3#StealerLogsByEmailDomain"></a>
    /// </summary>
    /// <param name="domain">The email domain to search for in stealer logs.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>A dictionary where the key is the email alias and the value is an array of website domains, or null if not found.</returns>
    /// <remarks>
    /// This search is by the domain of the email address, returning stealer log data.
    /// This API is normally used to identify individuals within an organization who've had credentials taken by an info stealer.
    /// Requires a Pwned 5 subscription or higher.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="domain"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<StealerLogsByEmailDomain?> GetStealerLogsByEmailDomainAsync(
        string domain,
        CancellationToken cancellationToken = default);
}
