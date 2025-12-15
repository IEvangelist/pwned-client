// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedClient
{
    /// <inheritdoc cref="IPwnedPasswordsClient.GetPwnedPasswordAsync(string, bool, CancellationToken)" />
    async Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordAsync(
        string plainTextPassword,
        bool addPadding,
        CancellationToken cancellationToken)
    {
        if (plainTextPassword is null or { Length: 0 })
        {
            throw new ArgumentException(
                "The plainTextPassword cannot be either null, or empty.", nameof(plainTextPassword));
        }

        var pwnedPassword = new PwnedPassword()
        {
            PlainTextPassword = plainTextPassword
        };

        if (pwnedPassword.IsInvalid())
        {
            return pwnedPassword;
        }

        try
        {
            var passwordHash = plainTextPassword.ToSha1Hash()!;

            var firstFiveChars = passwordHash[..5];

            var client = httpClientFactory.CreateClient(PasswordsClient);

            // Create request with optional padding header
            using var request = new HttpRequestMessage(HttpMethod.Get, $"range/{firstFiveChars}");
            if (addPadding)
            {
                request.Headers.Add("Add-Padding", "true");
            }

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var passwordHashesInRange = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            pwnedPassword = ParsePasswordRangeResponseText(
                pwnedPassword, passwordHashesInRange, passwordHash);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);
        }

        return pwnedPassword;
    }

    /// <inheritdoc cref="IPwnedPasswordsClient.GetPwnedPasswordWithNtlmAsync(string, bool, CancellationToken)" />
    async Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordWithNtlmAsync(
        string plainTextPassword,
        bool addPadding,
        CancellationToken cancellationToken)
    {
        if (plainTextPassword is null or { Length: 0 })
        {
            throw new ArgumentException(
                "The plainTextPassword cannot be either null, or empty.", nameof(plainTextPassword));
        }

        var pwnedPassword = new PwnedPassword()
        {
            PlainTextPassword = plainTextPassword
        };

        if (pwnedPassword.IsInvalid())
        {
            return pwnedPassword;
        }

        try
        {
            var passwordHash = plainTextPassword.ToNtlmHash()!;

            var firstFiveChars = passwordHash[..5];

            var client = httpClientFactory.CreateClient(PasswordsClient);

            // Create request with NTLM mode and optional padding header
            using var request = new HttpRequestMessage(HttpMethod.Get, $"range/{firstFiveChars}?mode=ntlm");
            if (addPadding)
            {
                request.Headers.Add("Add-Padding", "true");
            }

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var passwordHashesInRange = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            pwnedPassword = ParsePasswordRangeResponseText(
                pwnedPassword, passwordHashesInRange, passwordHash);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);
        }

        return pwnedPassword;
    }

    internal static readonly char[] s_newLineSeparator = ['\n'];
    internal static readonly char[] s_colonSeparator = [':'];

    internal static PwnedPassword ParsePasswordRangeResponseText(
        PwnedPassword pwnedPassword, string passwordRangeResponseText, string passwordHash)
    {
        pwnedPassword = pwnedPassword with
        {
            HashedPassword = passwordHash
        };

        if (passwordRangeResponseText is not null)
        {
            // Example passwordRangeResponseText
            // The remaining hash characters, less the first five separated by a : with the corresponding count.
            // <Remaining Hash>:<Count>

            // 0018A45C4D1DEF81644B54AB7F969B88D65:10
            // 00D4F6E8FA6EECAD2A3AA415EEC418D38EC:2
            // 011053FD0102E94D6AE2F8B83D76FAF94F6:701
            // 012A7CA357541F0AC487871FEEC1891C49C:2
            // 0136E006E24E7D152139815FB0FC6A50B15:2

            var hashCountMap = passwordRangeResponseText
                .Split(s_newLineSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(static hashCountPair =>
                {
                    var pair = hashCountPair
                        .Replace('\r', '\0')
                        .Split(s_colonSeparator, StringSplitOptions.RemoveEmptyEntries);

                    return pair?.Length != 2 || !long.TryParse(pair[1], out var count)
                        ? (Hash: "", Count: 0L, IsValid: false)
                        : (Hash: pair[0], Count: count, IsValid: true);
                })
                .Where(static t => t.IsValid)
                .ToFrozenDictionary(static t => t.Hash, t => t.Count, StringComparer.OrdinalIgnoreCase);

            var hashSuffix = passwordHash[5..];
            if (hashCountMap.TryGetValue(hashSuffix, out var count))
            {
                pwnedPassword = pwnedPassword with
                {
                    PwnedCount = count,
                    IsPwned = count > 0,
                };
            }
        }

        return pwnedPassword;
    }
}
