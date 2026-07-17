// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient
{
    async Task<BreachDetails?> IPwnedBreachesClient.GetBreachAsync(
        string breachName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(breachName);

        return await GetJsonAsync(
                HibpClient,
                $"breach/{EscapePathSegment(breachName)}",
                SourceGeneratorContext.Default.BreachDetails,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                BuildBreachCatalogueUri(domain, isSpamList),
                SourceGeneratorContext.Default.BreachHeaderArray,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false) ?? [];

    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachesAsAsyncEnumerable(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken) =>
        GetJsonArrayAsync(
            HibpClient,
            BuildBreachCatalogueUri(domain, isSpamList),
            SourceGeneratorContext.Default.BreachHeader,
            notFoundIsEmpty: false,
            cancellationToken);

    async Task<BreachDetails[]> IPwnedBreachesClient.GetAllBreachDetailsAsync(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                BuildBreachCatalogueUri(domain, isSpamList),
                SourceGeneratorContext.Default.BreachDetailsArray,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false) ?? [];

    IAsyncEnumerable<BreachDetails?> IPwnedBreachesClient.GetAllBreachDetailsAsAsyncEnumerable(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken) =>
        GetJsonArrayAsync(
            HibpClient,
            BuildBreachCatalogueUri(domain, isSpamList),
            SourceGeneratorContext.Default.BreachDetails,
            notFoundIsEmpty: false,
            cancellationToken);

    async Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return await GetJsonAsync(
                HibpClient,
                BuildBreachedAccountUri(
                    account,
                    truncateResponse: false,
                    includeUnverified,
                    domain),
                SourceGeneratorContext.Default.BreachDetailsArray,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    IAsyncEnumerable<BreachDetails?> IPwnedBreachesClient.GetBreachesForAccountAsAsyncEnumerable(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return GetJsonArrayAsync(
            HibpClient,
            BuildBreachedAccountUri(
                account,
                truncateResponse: false,
                includeUnverified,
                domain),
            SourceGeneratorContext.Default.BreachDetails,
            notFoundIsEmpty: true,
            cancellationToken);
    }

    Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(
        string account,
        CancellationToken cancellationToken) =>
        ((IPwnedBreachesClient)this).GetBreachHeadersForAccountAsync(
            account,
            includeUnverified: true,
            domain: null,
            cancellationToken);

    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return await GetJsonAsync(
                HibpClient,
                BuildBreachedAccountUri(
                    account,
                    truncateResponse: true,
                    includeUnverified,
                    domain),
                SourceGeneratorContext.Default.BreachHeaderArray,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachHeadersForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken) =>
        ((IPwnedBreachesClient)this).GetBreachHeadersForAccountAsAsyncEnumerable(
            account,
            includeUnverified: true,
            domain: null,
            cancellationToken);

    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachHeadersForAccountAsAsyncEnumerable(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        return GetJsonArrayAsync(
            HibpClient,
            BuildBreachedAccountUri(
                account,
                truncateResponse: true,
                includeUnverified,
                domain),
            SourceGeneratorContext.Default.BreachHeader,
            notFoundIsEmpty: true,
            cancellationToken);
    }

    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountUsingKAnonymityAsync(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        var normalizedAccount = account.Trim().ToLowerInvariant();
        var accountHash = normalizedAccount.ToSha1Hash()!;
        var hashPrefix = accountHash[..6];
        var hashSuffix = accountHash[6..];

        await foreach (var candidate in GetJsonArrayAsync(
            HibpClient,
            $"breachedaccount/range/{hashPrefix}",
            InternalSourceGeneratorContext.Default.BreachedAccountHashRange,
            notFoundIsEmpty: false,
            cancellationToken).ConfigureAwait(false))
        {
            if (candidate is not null &&
                string.Equals(
                    candidate.HashSuffix,
                    hashSuffix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return candidate.Websites
                    .Select(static website => new BreachHeader { Name = website })
                    .ToArray();
            }
        }

        return [];
    }

    async IAsyncEnumerable<BreachHeader?>
        IPwnedBreachesClient.GetBreachHeadersForAccountUsingKAnonymityAsAsyncEnumerable(
            string account,
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken)
    {
        var breaches = await ((IPwnedBreachesClient)this)
            .GetBreachHeadersForAccountUsingKAnonymityAsync(account, cancellationToken)
            .ConfigureAwait(false);

        foreach (var breach in breaches)
        {
            yield return breach;
        }
    }

    async Task<string[]> IPwnedBreachesClient.GetDataClassesAsync(
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                "dataclasses",
                SourceGeneratorContext.Default.StringArray,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false) ?? [];

    IAsyncEnumerable<string?> IPwnedBreachesClient.GetDataClassesAsAsyncEnumerable(
        CancellationToken cancellationToken) =>
        GetJsonArrayAsync(
            HibpClient,
            "dataclasses",
            SourceGeneratorContext.Default.String,
            notFoundIsEmpty: false,
            cancellationToken);

    async Task<BreachDetails?> IPwnedBreachesClient.GetLatestBreachAsync(
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                "latestbreach",
                SourceGeneratorContext.Default.BreachDetails,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);

    private static string BuildBreachCatalogueUri(
        string? domain,
        bool? isSpamList) =>
        BuildRequestUri(
            "breaches",
            ("domain", string.IsNullOrWhiteSpace(domain) ? null : domain.Trim()),
            ("isSpamList", isSpamList?.ToString().ToLowerInvariant()));

    private static string BuildBreachedAccountUri(
        string account,
        bool truncateResponse,
        bool includeUnverified,
        string? domain) =>
        BuildRequestUri(
            $"breachedaccount/{EscapePathSegment(account)}",
            ("truncateResponse", truncateResponse.ToString().ToLowerInvariant()),
            ("includeUnverified", includeUnverified.ToString().ToLowerInvariant()),
            ("domain", string.IsNullOrWhiteSpace(domain) ? null : domain.Trim()));
}
