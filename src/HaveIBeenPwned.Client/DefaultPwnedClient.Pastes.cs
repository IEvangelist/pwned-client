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
            var client = httpClientFactory.CreateClient(HibpClient);
            var pastes =
                await client.GetFromJsonAsync<Pastes[]>(
                    $"pasteaccount/{HttpUtility.UrlEncode(account)}");

            return pastes ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc cref="IPwnedPastesClient.GetPastesAsAsyncEnumerable(string, CancellationToken)" />
    IAsyncEnumerable<Pastes?> IPwnedPastesClient.GetPastesAsAsyncEnumerable(
        string account, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            throw new ArgumentException(
                "The account cannot be either null, empty or whitespace.", nameof(account));
        }

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);
            
                return client.GetFromJsonAsAsyncEnumerable<Pastes>(
                    $"pasteaccount/{HttpUtility.UrlEncode(account)}",
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return AsyncEnumerable.Empty<Pastes?>();
        }
    }
}
