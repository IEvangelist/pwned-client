// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedClient
{
    async Task<Pastes[]> IPwnedPastesClient.GetPastesAsync(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return await GetJsonAsync(
                HibpClient,
                $"pasteaccount/{EscapePathSegment(account)}",
                SourceGeneratorContext.Default.PastesArray,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    IAsyncEnumerable<Pastes?> IPwnedPastesClient.GetPastesAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return GetJsonArrayAsync(
            HibpClient,
            $"pasteaccount/{EscapePathSegment(account)}",
            SourceGeneratorContext.Default.Pastes,
            notFoundIsEmpty: true,
            cancellationToken);
    }
}
