// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient
{
    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachAsync(string, CancellationToken)" />
    async Task<BreachDetails?> IPwnedBreachesClient.GetBreachAsync(
        string breachName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(breachName);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails>(
                        $"breach/{breachName}",
                        SourceGeneratorContext.Default.BreachDetails,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return breachDetails;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesAsync(string?, CancellationToken)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(
        string? domain,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryString = string.IsNullOrWhiteSpace(domain)
                ? ""
                : $"?domain={domain}";

            var breachHeaders =
                await client.GetFromJsonAsync<BreachHeader[]>(
                        $"breaches{queryString}",
                        SourceGeneratorContext.Default.BreachHeaderArray,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return breachHeaders ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc path="IPwnedBreachesClient.GetBreachesAsAsyncEnumerable(string?, CancellationToken)" />
    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachesAsAsyncEnumerable(
        string? domain,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryString = string.IsNullOrWhiteSpace(domain)
                ? ""
                : $"?domain={domain}";

            return client.GetFromJsonAsAsyncEnumerable<BreachHeader>(
                    $"breaches{queryString}",
                    SourceGeneratorContext.Default.BreachHeader,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return AsyncEnumerable.Empty<BreachHeader?>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesForAccountAsync(string, CancellationToken)" />
    async Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails[]>(
                        $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=false",
                        SourceGeneratorContext.Default.BreachDetailsArray,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return breachDetails ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc path="IPwnedBreachesClient.GetBreachesForAccountAsAsyncEnumerable(string, CancellationToken)" />"
    IAsyncEnumerable<BreachDetails?> IPwnedBreachesClient.GetBreachesForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            return client.GetFromJsonAsAsyncEnumerable<BreachDetails>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=false",
                    SourceGeneratorContext.Default.BreachDetails,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return AsyncEnumerable.Empty<BreachDetails?>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string, CancellationToken)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachHeadersForAccountAsync(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachHeaders = await client.GetFromJsonAsync<BreachHeader[]>(
                $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=true",
                SourceGeneratorContext.Default.BreachHeaderArray,
                cancellationToken);

            return breachHeaders ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc path="IPwnedBreachesClient.GetBreachHeadersForAccountAsAsyncEnumerable(string, CancellationToken)" />
    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachHeadersForAccountAsAsyncEnumerable(
        string account,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            return client.GetFromJsonAsAsyncEnumerable<BreachHeader>(
                $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=true",
                SourceGeneratorContext.Default.BreachHeader,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return AsyncEnumerable.Empty<BreachHeader?>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetDataClassesAsync(CancellationToken)" />
    async Task<string[]> IPwnedBreachesClient.GetDataClassesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var dataClasses =
                await client.GetFromJsonAsync<string[]>(
                        "dataclasses",
                        SourceGeneratorContext.Default.StringArray,
                        cancellationToken: cancellationToken
                    )
                    .ConfigureAwait(false);

            return dataClasses ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc path="IPwnedBreachesClient.GetDataClassesAsAsyncEnumerable(CancellationToken)" />
    IAsyncEnumerable<string?> IPwnedBreachesClient.GetDataClassesAsAsyncEnumerable(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            return client.GetFromJsonAsAsyncEnumerable<string>(
                    "dataclasses",
                    SourceGeneratorContext.Default.String,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return AsyncEnumerable.Empty<string?>();
        }
    }
}