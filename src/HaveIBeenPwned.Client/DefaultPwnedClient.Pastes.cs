// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedClient
{
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
}
