// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// The "pwned" breaches client.
/// </summary>
public interface IPwnedBreachesClient
{
    /// <summary>
    /// The API takes a single parameter which is the account to be searched for.
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces.
    /// The account should always be URL encoded.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForAccount"></a>
    /// </summary>
    /// <param name="account">The account to search for breaches.</param>
    /// <returns>An array of breach headers if found, or an empty array if not found.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>true</c>, causing only the breach headers to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is either <c>null</c>, empty or whitespace.
    /// </exception>
    Task<BreachHeader[]> GetBreachHeadersForAccountAsync(string account);

    /// <summary>
    /// The API takes a single parameter which is the account to be searched for.
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces.
    /// The account should always be URL encoded.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForAccount"></a>
    /// </summary>
    /// <param name="account">The account to search for breaches.</param>
    /// <returns>An array of breach details if found, or an empty array if not found.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>false</c>, allows breach details to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is <c>null</c>, empty or whitespace.
    /// </exception>
    Task<BreachDetails[]> GetBreachesForAccountAsync(string account);

    /// <summary>
    /// Gets an array of <see cref="BreachHeader"/>, optionally filtering on <paramref name="domain"/>.
    /// </summary>
    /// <param name="domain">An optional domain to filter the returned breaches to.</param>
    /// <returns>An array of breach headers, or an empty array.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/breaches"></a>
    /// </remarks>
    Task<BreachHeader[]> GetBreachesAsync(string? domain = default);

    /// <summary>
    /// Gets the <see cref="BreachDetails"/> object for the given <paramref name="breachName"/>.
    /// </summary>
    /// <param name="breachName">The name of the breach to get.</param>
    /// <returns>The breach details for the given <paramref name="breachName"/>, if unable to find one <c>null</c> is returned.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/breach/Adobe"></a>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="breachName"/> is <c>null</c>, empty or whitespace.
    /// </exception>
    Task<BreachDetails?> GetBreachAsync(string breachName);

    /// <summary>
    /// Gets all of the data classes possible for any given <see cref="BreachDetails.DataClasses"/>.
    /// </summary>
    /// <returns>An array of all the possible data classes, or an empty array.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/dataclasses"></a>
    /// </remarks>
    Task<string[]> GetDataClassesAsync();
}
