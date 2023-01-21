// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <inheritdoc cref="DefaultPwnedClient" />
public sealed class PwnedClient : IPwnedClient
{
    private readonly IPwnedClient _pwnedClient;

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
    /// <remarks>
    /// This is not intended for usage in conjunction with dependency injection
    /// (DI). In other words, if you don't want to use DI you can use this instead.
    /// </remarks>
    /// <returns>
    /// An <see cref="IPwnedClient"/> implementation from the given
    /// <paramref name="apiKey"/> value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the given <paramref name="apiKey"/> value is <c>null</c>, this exception is thrown.
    /// </exception>
    public PwnedClient(string apiKey) =>
        _pwnedClient = PwnedClientFactory.FromApiKey(apiKey);

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachAsync(string)"/>
    Task<BreachDetails?> IPwnedBreachesClient.GetBreachAsync(string breachName) =>
        _pwnedClient.GetBreachAsync(breachName);

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesAsync(string?)"/>
    Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(string? domain) =>
        _pwnedClient.GetBreachesAsync(domain);

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesForAccountAsync(string)"/>
    Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(string account) =>
        _pwnedClient.GetBreachesForAccountAsync(account);

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string)"/>
    Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string account) =>
        _pwnedClient.GetBreachHeadersForAccountAsync(account);

    /// <inheritdoc cref="IPwnedBreachesClient.GetDataClassesAsync"/>
    Task<string[]> IPwnedBreachesClient.GetDataClassesAsync() =>
        _pwnedClient.GetDataClassesAsync();

    /// <inheritdoc cref="IPwnedPastesClient.GetPastesAsync(string)"/>
    Task<Pastes[]> IPwnedPastesClient.GetPastesAsync(string account) =>
        _pwnedClient.GetPastesAsync(account);

    /// <inheritdoc cref="IPwnedPasswordsClient.GetPwnedPasswordAsync(string)"/>
    Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordAsync(string plainTextPassword) =>
        _pwnedClient.GetPwnedPasswordAsync(plainTextPassword);
}
