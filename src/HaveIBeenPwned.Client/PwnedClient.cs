// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// Creates and returns an <see cref="PwnedClient"/> from the given
/// <paramref name="apiKey"/> value. Consider the following example usage:
/// <code language="csharp">
/// IPwnedClient client = new PwnedClient(configuration["ApiKey"]);
/// // TODO: use client...
/// </code>
/// </summary>
/// <param name="apiKey">
/// The API key, used to authorize HTTP calls to HIBP.
/// See <a href="https://haveibeenpwned.com/api/v3#Authorisation"></a>
/// </param>
/// <param name="loggerFactory">
/// An optional <see cref="ILoggerFactory"/> to create the
/// <see cref="ILogger"/> used within the <see cref="PwnedClient"/> instance
/// </param>
/// <remarks>
/// This is not intended for usage in conjunction with dependency injection
/// (DI). In other words, if you don't want to use DI you can use this instead.
/// </remarks>
/// <returns>
/// An <see cref="IPwnedClient"/> implementation from the given
/// <paramref name="apiKey"/> value.
/// </returns>
/// <exception cref="ArgumentNullException">
/// If the given <paramref name="apiKey"/> value is <see langword="null" />, this exception is thrown.
/// </exception>
public sealed class PwnedClient(string apiKey, ILoggerFactory? loggerFactory = default) : IPwnedClient
{
    private readonly IPwnedClient _pwnedClient = PwnedClientFactory.FromApiKey(apiKey, loggerFactory);

    /// <inheritdoc/>
    Task<SubscriptionStatus?> IPwnedClient.GetSubscriptionStatusAsync(
        CancellationToken cancellationToken) => _pwnedClient.GetSubscriptionStatusAsync(cancellationToken);

    /// <inheritdoc/>
    Task<BreachDetails?> IPwnedBreachesClient.GetBreachAsync(
        string breachName,
        CancellationToken cancellationToken) => _pwnedClient.GetBreachAsync(breachName, cancellationToken);

    /// <inheritdoc/>
    Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(
        string? domain,
        CancellationToken cancellationToken) => _pwnedClient.GetBreachesAsync(domain, cancellationToken);

    /// <inheritdoc/>
    Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(
        string account,
        CancellationToken cancellationToken) => _pwnedClient.GetBreachesForAccountAsync(account, cancellationToken);

    /// <inheritdoc/>
    Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(
        string account,
        CancellationToken cancellationToken) => _pwnedClient.GetBreachHeadersForAccountAsync(account, cancellationToken);

    /// <inheritdoc/>
    Task<string[]> IPwnedBreachesClient.GetDataClassesAsync(CancellationToken cancellationToken) => _pwnedClient.GetDataClassesAsync(cancellationToken);

    /// <inheritdoc/>
    Task<Pastes[]> IPwnedPastesClient.GetPastesAsync(
        string account,
        CancellationToken cancellationToken) => _pwnedClient.GetPastesAsync(account, cancellationToken);

    /// <inheritdoc/>
    Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordAsync(
        string plainTextPassword,
        CancellationToken cancellationToken) => _pwnedClient.GetPwnedPasswordAsync(plainTextPassword, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<BreachHeader?> GetBreachesAsAsyncEnumerable(
        string? domain = null,
        CancellationToken cancellationToken = default) => _pwnedClient.GetBreachesAsAsyncEnumerable(domain, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<BreachDetails?> GetBreachesForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken = default) => _pwnedClient.GetBreachesForAccountAsAsyncEnumerable(account, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<BreachHeader?> GetBreachHeadersForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken = default) => _pwnedClient.GetBreachHeadersForAccountAsAsyncEnumerable(account, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<string?> GetDataClassesAsAsyncEnumerable(
        CancellationToken cancellationToken = default) => _pwnedClient.GetDataClassesAsAsyncEnumerable(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<Pastes?> GetPastesAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken) => _pwnedClient.GetPastesAsAsyncEnumerable(account, cancellationToken);
}
