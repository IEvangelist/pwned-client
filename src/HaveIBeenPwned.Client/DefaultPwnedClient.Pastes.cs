// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedClient
{
    /// <inheritdoc cref="IPwnedPastesClient.GetPastesAsync(string, CancellationToken)" />
    async Task<Pastes[]> IPwnedPastesClient.GetPastesAsync(
        string account,
        CancellationToken cancellationToken)
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
                await client.GetFromJsonAsync<Pastes[]>($"pasteaccount/{HttpUtility.UrlEncode(account)}", SourceGeneratorContext.Default.PastesArray, cancellationToken: cancellationToken);

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
        string account,
        CancellationToken cancellationToken)
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
                SourceGeneratorContext.Default.Pastes,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return Internals.AsyncEnumerable.Empty<Pastes?>();
        }
    }
}
