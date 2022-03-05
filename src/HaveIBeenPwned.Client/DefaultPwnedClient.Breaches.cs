// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient
{
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
}