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
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails>(
                    $"breach/{breachName}",
                    SourceGeneratorContext.Default.BreachDetails);

            return breachDetails;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesAsync(string?)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(string? domain)
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
                    SourceGeneratorContext.Default.BreachHeaderArray);

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
        string? domain, CancellationToken cancellationToken)
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
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails[]>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=false",
                    SourceGeneratorContext.Default.BreachDetailsArray);

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
        string account, CancellationToken cancellationToken)
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
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachHeaders =
                await client.GetFromJsonAsync<BreachHeader[]>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?truncateResponse=true",
                    SourceGeneratorContext.Default.BreachHeaderArray);

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
                    cancellationToken: cancellationToken);

            return dataClasses ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc path="IPwnedBreachesClient.GetDataClassesAsAsyncEnumerable" />
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