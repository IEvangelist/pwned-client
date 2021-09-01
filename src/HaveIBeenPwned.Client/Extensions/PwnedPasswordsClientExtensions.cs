// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;

namespace HaveIBeenPwned.Client.Extensions
{
    /// <summary></summary>
    public static class PwnedPasswordsClientExtensions
    {
        /// <summary>
        /// An extension method that evaluates whether the <paramref name="plainTextPassword"/> is "pwned".
        /// When <c>true</c>, the <c>Count</c> is at least <c>1</c>.
        /// </summary>
        /// <param name="pwnedPasswordsClient"></param>
        /// <param name="plainTextPassword"></param>
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
        public static async ValueTask<(bool? IsPwned, long? Count)> IsPasswordPwnedAsync(
            this IPwnedPasswordsClient pwnedPasswordsClient, string plainTextPassword)
        {
            var pwnedPassword = await pwnedPasswordsClient.GetPwnedPasswordAsync(plainTextPassword);

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
        /// <param name="pwnedBreachesClient"></param>
        /// <param name="account"></param>
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
        public static async ValueTask<(bool? IsBreached, string[]? Breaches)> IsBreachedAccountAsync(
            this IPwnedBreachesClient pwnedBreachesClient, string account)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return (null, null);
            }

            var breaches = await pwnedBreachesClient.GetBreachHeadersForAccountAsync(account);

            return
                (
                    IsBreached: breaches is { Length: > 0 },
                    Breaches: breaches?.Select(breach => breach.Name)?.ToArray() ?? Array.Empty<string>()
                );
        }
    }
}
