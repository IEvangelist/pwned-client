// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// An aggregate interface for convenience, exposing all
/// "pwned" functionality including:
/// <list type="bullet">
/// <item><see cref="IPwnedBreachesClient"/></item>
/// <item><see cref="IPwnedPastesClient"/></item>
/// <item><see cref="IPwnedPasswordsClient"/></item>
/// </list>
/// </summary>
public interface IPwnedClient : IPwnedBreachesClient,
                                IPwnedPastesClient,
                                IPwnedPasswordsClient
{
    /// <summary>
    /// Gets the <see cref="SubscriptionStatus"/> representing current status
    /// of the subscription for the API key used to instantiate the client.
    /// See <a href="https://haveibeenpwned.com/API/v3#SubscriptionStatus"></a>
    /// </summary>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    Task<SubscriptionStatus?> GetSubscriptionStatusAsync(
        CancellationToken cancellationToken = default);
}
