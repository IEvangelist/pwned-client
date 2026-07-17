// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// The "pwned" domain client for domain search operations.
/// </summary>
public interface IPwnedDomainClient
{
    /// <summary>
    /// Generates the DNS TXT record used to verify control of a domain.
    /// </summary>
    /// <param name="domain">The domain to verify.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>The DNS TXT record value to publish.</returns>
    Task<DomainVerificationDnsToken> GenerateDomainVerificationDnsTokenAsync(
        string domain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the HIBP DNS TXT record published for a domain.
    /// </summary>
    /// <param name="domain">The domain to verify.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    Task VerifyDomainViaDnsAsync(
        string domain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a domain verification message to one of the supported email aliases.
    /// </summary>
    /// <param name="domain">The domain to verify.</param>
    /// <param name="emailAlias">The email alias that should receive the verification message.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    Task SendDomainVerificationEmailAsync(
        string domain,
        DomainVerificationEmailAlias emailAlias,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all breached email addresses on a given domain and the breaches they've appeared in.
    /// Only domains that have been successfully added to the domain search dashboard after verifying control can be searched.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForDomain"></a>
    /// </summary>
    /// <param name="domain">The domain to search for breached email addresses.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>A dictionary where the key is the email alias and the value is an array of breach names, or null if not found.</returns>
    /// <remarks>
    /// This is an authenticated API requiring an HIBP API key.
    /// Note: the domain search API will return sensitive data breaches as it can only be called after demonstrating control of the domain.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="domain"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<DomainBreaches?> GetBreachedDomainAsync(
        string domain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all domains that have been successfully added to the domain search dashboard after verifying control.
    /// See <a href="https://haveibeenpwned.com/API/v3#SubscribedDomains"></a>
    /// </summary>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of subscribed domains associated with the API key, or an empty array.</returns>
    /// <remarks>
    /// This is an authenticated API requiring an HIBP API key which will then return all domains associated with that key.
    /// </remarks>
    Task<SubscribedDomain[]> GetSubscribedDomainsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asynchronous stream of subscribed domains.
    /// See <a href="https://haveibeenpwned.com/API/v3#SubscribedDomains"></a>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>An asynchronous stream of <see cref="SubscribedDomain"/> objects.</returns>
    /// <remarks>
    /// This is an authenticated API requiring an HIBP API key which will then return all domains associated with that key.
    /// </remarks>
    IAsyncEnumerable<SubscribedDomain?> GetSubscribedDomainsAsAsyncEnumerable(
        CancellationToken cancellationToken = default);
}
