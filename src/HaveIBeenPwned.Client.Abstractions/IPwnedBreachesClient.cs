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
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of breach headers if found, or an empty array if not found.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>true</c>, causing only the breach headers to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is either <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<BreachHeader[]> GetBreachHeadersForAccountAsync(
        string account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asynchronous stream of <see cref="BreachHeader"/> objects representing all known data breaches that include the specified account.
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces.
    /// The account should always be URL encoded.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForAccount"></a>
    /// </summary>
    /// <param name="account">The account to search for in the data breaches.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>An asynchronous stream of <see cref="BreachHeader"/> objects representing all known data breaches that include the specified account.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>true</c>, causing only the breach headers to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is either <see langword="null" />, empty or whitespace.
    /// </exception>
    IAsyncEnumerable<BreachHeader?> GetBreachHeadersForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// The API takes a single parameter which is the account to be searched for.
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces.
    /// The account should always be URL encoded.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForAccount"></a>
    /// </summary>
    /// <param name="account">The account to search for breaches.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of breach details if found, or an empty array if not found.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>false</c>, allows breach details to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<BreachDetails[]> GetBreachesForAccountAsync(
        string account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of all breaches that a particular account has been involved in, as an asynchronous enumerable.
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces.
    /// The account should always be URL encoded.
    /// See <a href="https://haveibeenpwned.com/API/v3#BreachesForAccount"></a>
    /// </summary>
    /// <param name="account">The account to retrieve breaches for.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An asynchronous enumerable of <see cref="BreachDetails"/> objects representing the breaches that the account has been involved in.</returns>
    /// <remarks>
    /// The <c>truncateResponse</c> is set to <c>false</c>, allows breach details to be returned.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="account"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    IAsyncEnumerable<BreachDetails?> GetBreachesForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an array of <see cref="BreachHeader"/>, optionally filtering on <paramref name="domain"/>.
    /// </summary>
    /// <param name="domain">An optional domain to filter the returned breaches to.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>An array of breach headers, or an empty array.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/breaches"></a>
    /// </remarks>
    Task<BreachHeader[]> GetBreachesAsync(
        string? domain = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of breach headers as an asynchronous enumerable, optionally filtering on <paramref name="domain"/>.
    /// </summary>
    /// <param name="domain">An optional domain to filter the returned breaches to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An asynchronous enumerable of <see cref="BreachHeader"/> objects.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/breaches"></a>
    /// </remarks>
    IAsyncEnumerable<BreachHeader?> GetBreachesAsAsyncEnumerable(
        string? domain = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="BreachDetails"/> object for the given <paramref name="breachName"/>.
    /// </summary>
    /// <param name="breachName">The name of the breach to get.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>The breach details for the given <paramref name="breachName"/>, if unable to find one <see langword="null" /> is returned.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/breach/Adobe"></a>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// When the <paramref name="breachName"/> is <see langword="null" />, empty or whitespace.
    /// </exception>
    Task<BreachDetails?> GetBreachAsync(
        string breachName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all of the data classes possible for any given <see cref="BreachDetails.DataClasses"/>.
    /// </summary>
    /// <returns>An array of all the possible data classes, or an empty array.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/dataclasses"></a>
    /// </remarks>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    Task<string[]> GetDataClassesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an asynchronous enumerable of all data classes in the Have I Been Pwned database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An asynchronous enumerable of strings representing the data classes.</returns>
    /// <remarks>
    /// Example JSON payload: <a href="https://haveibeenpwned.com/api/v3/dataclasses"></a>
    /// </remarks>
    IAsyncEnumerable<string?> GetDataClassesAsAsyncEnumerable(CancellationToken cancellationToken = default);
}
