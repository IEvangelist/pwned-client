// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// Provides extension methods for the <see cref="IPwnedClient"/> interface.
/// </summary>
public static class PwnedClientExtensions
{
    /// <summary>
    /// An extension method that evaluates whether the <paramref name="plainTextPassword"/> is "pwned".
    /// When <c>true</c>, the <c>Count</c> is at least <c>1</c>.
    /// </summary>
    /// <param name="pwnedPasswordsClient">The client instance that this method extends.</param>
    /// <param name="plainTextPassword">The plaint text password to evaluate.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item>
    /// When the given <paramref name="plainTextPassword"/> is "pwned", returns <c>(true, 3)</c> when "pwned" three times.
    /// </item>
    /// <item>
    /// When the given <paramref name="plainTextPassword"/> <strong>isn't</strong> "pwned", this could return <c>(false, 0)</c>.
    /// </item>
    /// <item>
    /// When unable to determine, returns <c>(null, null)</c>.
    /// </item>
    /// </list>
    /// </returns>
    public static async Task<(bool? IsPwned, long? Count)> IsPasswordPwnedAsync(
        this IPwnedPasswordsClient pwnedPasswordsClient,
        string plainTextPassword,
        CancellationToken cancellationToken = default)
    {
        var pwnedPassword = await pwnedPasswordsClient.GetPwnedPasswordAsync(
            plainTextPassword, cancellationToken);

        return
            (
                IsPwned: pwnedPassword.IsPwned ?? false,
                Count: pwnedPassword.PwnedCount
            );
    }

    /// <summary>
    /// An extension method that evaluates whether the <paramref name="account"/> is part of a breach.
    /// When <c>true</c>, the <c>Breaches</c> has at least one breach name.
    /// </summary>
    /// <param name="pwnedBreachesClient">The client instance that this method extends.</param>
    /// <param name="account">The account to evaluate.</param>
    /// <param name="cancellationToken">Used to signal cancellation.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item>
    /// When the given <paramref name="account"/> is part of a breach, returns
    /// <c>(true, ["Adobe", "LinkedIn"])</c> when the found in the Adobe and LinkedIn breaches.
    /// </item>
    /// <item>
    /// When the given <paramref name="account"/> <strong>isn't</strong> part of a breach, returns <c>(false, [])</c>.
    /// </item>
    /// <item>
    /// When unable to determine, returns <c>(null, null)</c>.
    /// </item>
    /// </list>
    /// </returns>
    public static async Task<(bool? IsBreached, string[]? Breaches)> IsBreachedAccountAsync(
        this IPwnedBreachesClient pwnedBreachesClient,
        string account,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            return (null, null);
        }

        var breaches = await pwnedBreachesClient.GetBreachHeadersForAccountAsync(
            account, cancellationToken);

        return
            (
                IsBreached: breaches is { Length: > 0 },
                Breaches: breaches?.Select(breach => breach.Name)?.ToArray() ?? []
            );
    }
}
