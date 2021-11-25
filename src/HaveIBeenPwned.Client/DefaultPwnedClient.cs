// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal class DefaultPwnedClient : IPwnedClient
{
    readonly IHttpClientFactory _httpClientFactory;
    readonly ILogger<DefaultPwnedClient> _logger;

    public DefaultPwnedClient(
        IHttpClientFactory httpClientFactory,
        ILogger<DefaultPwnedClient> logger) =>
        (_httpClientFactory, _logger) = (httpClientFactory, logger);

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachAsync(string)" />
    async Task<BreachDetails?> IPwnedBreachesClient.GetBreachAsync(string breachName)
    {
        if (string.IsNullOrWhiteSpace(breachName))
        {
            throw new ArgumentException(
                "The breachName cannot be either null, empty or whitespace.", nameof(breachName));
        }

        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails>(
                    $"breach/{breachName}");

            return breachDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesAsync(string?)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(string? domain)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var queryString = string.IsNullOrWhiteSpace(domain)
                ? ""
                : $"?domain={domain}";

            var breachHeaders =
                await client.GetFromJsonAsync<BreachHeader[]>(
                    $"breaches{queryString}");

            return breachHeaders ?? Array.Empty<BreachHeader>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Array.Empty<BreachHeader>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesForAccountAsync(string)" />
    async Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(string account)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            throw new ArgumentException(
                "The account cannot be either null, empty or whitespace.", nameof(account));
        }

        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails[]>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=false");

            return breachDetails ?? Array.Empty<BreachDetails>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Array.Empty<BreachDetails>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string account)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            throw new ArgumentException(
                "The account cannot be either null, empty or whitespace.", nameof(account));
        }

        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails[]>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=true");

            return breachDetails ?? Array.Empty<BreachDetails>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Array.Empty<BreachDetails>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetDataClassesAsync" />
    async Task<string[]> IPwnedBreachesClient.GetDataClassesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var dataClasses =
                await client.GetFromJsonAsync<string[]>("dataclasses");

            return dataClasses ?? Array.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Array.Empty<string>();
        }
    }

    /// <inheritdoc cref="IPwnedPastesClient.GetPastesAsync(string)" />
    async Task<Pastes[]> IPwnedPastesClient.GetPastesAsync(string account)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            throw new ArgumentException(
                "The account cannot be either null, empty or whitespace.", nameof(account));
        }

        try
        {
            var client = _httpClientFactory.CreateClient(HibpClient);
            var pastes =
                await client.GetFromJsonAsync<Pastes[]>(
                    $"pasteaccount/{HttpUtility.UrlEncode(account)}");

            return pastes ?? Array.Empty<Pastes>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Array.Empty<Pastes>();
        }
    }

    /// <inheritdoc cref="IPwnedPasswordsClient.GetPwnedPasswordAsync(string)" />
    async Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordAsync(string plainTextPassword)
    {
        if (plainTextPassword is null or { Length: 0 })
        {
            throw new ArgumentException(
                "The plainTextPassword cannot be either null, or empty.", nameof(plainTextPassword));
        }

        var pwnedPassword = new PwnedPassword(plainTextPassword);
        if (pwnedPassword.IsInvalid())
        {
            return pwnedPassword;
        }

        try
        {
            var passwordHash = plainTextPassword.ToSha1Hash()!;
            var firstFiveChars = passwordHash[..5];
            var client = _httpClientFactory.CreateClient(PasswordsClient);
            var passwordHashesInRange =
                await client.GetStringAsync($"range/{firstFiveChars}");

            pwnedPassword =
                ParsePasswordRangeResponseText(pwnedPassword, passwordHashesInRange, passwordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return pwnedPassword;
    }

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

            var hashCountMap =
                passwordRangeResponseText
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(hashCountPair =>
                    {
                        var pair = hashCountPair
                            .Replace('\r', '\0')
                            .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        return pair?.Length != 2 || !long.TryParse(pair[1], out var count)
                            ? (Hash: "", Count: 0L, IsValid: false)
                            : (Hash: pair[0], Count: count, IsValid: true);
                    })
                    .Where(t => t.IsValid)
                    .ToDictionary(t => t.Hash, t => t.Count, StringComparer.OrdinalIgnoreCase);

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
