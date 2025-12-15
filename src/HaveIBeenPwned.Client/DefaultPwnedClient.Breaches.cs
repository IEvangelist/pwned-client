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

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesAsync(string?, bool?, CancellationToken)" />
    async Task<BreachHeader[]> IPwnedBreachesClient.GetBreachesAsync(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(domain))
            {
                queryParams.Add($"domain={domain}");
            }
            if (isSpamList.HasValue)
            {
                queryParams.Add($"isSpamList={isSpamList.Value.ToString().ToLowerInvariant()}");
            }

            var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";

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

    /// <inheritdoc path="IPwnedBreachesClient.GetBreachesAsAsyncEnumerable(string?, bool?, CancellationToken)" />
    IAsyncEnumerable<BreachHeader?> IPwnedBreachesClient.GetBreachesAsAsyncEnumerable(
        string? domain,
        bool? isSpamList,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(domain))
            {
                queryParams.Add($"domain={domain}");
            }
            if (isSpamList.HasValue)
            {
                queryParams.Add($"isSpamList={isSpamList.Value.ToString().ToLowerInvariant()}");
            }

            var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";

            return client.GetFromJsonAsAsyncEnumerable<BreachHeader>(
                    $"breaches{queryString}",
                    SourceGeneratorContext.Default.BreachHeader,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return Internals.AsyncEnumerable.Empty<BreachHeader?>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachesForAccountAsync(string, bool, string?, CancellationToken)" />
    async Task<BreachDetails[]> IPwnedBreachesClient.GetBreachesForAccountAsync(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(account);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryParams = new List<string> { "truncateResponse=false" };
            if (!includeUnverified)
            {
                queryParams.Add("includeUnverified=false");
            }
            if (!string.IsNullOrWhiteSpace(domain))
            {
                queryParams.Add($"domain={domain}");
            }

            var queryString = string.Join("&", queryParams);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails[]>(
                        $"breachedaccount/{HttpUtility.UrlEncode(account)}?{queryString}",
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

    /// <inheritdoc path="IPwnedBreachesClient.GetBreachesForAccountAsAsyncEnumerable(string, bool, string?, CancellationToken)" />
    IAsyncEnumerable<BreachDetails?> IPwnedBreachesClient.GetBreachesForAccountAsAsyncEnumerable(
        string account,
        bool includeUnverified,
        string? domain,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var queryParams = new List<string> { "truncateResponse=false" };
            if (!includeUnverified)
            {
                queryParams.Add("includeUnverified=false");
            }
            if (!string.IsNullOrWhiteSpace(domain))
            {
                queryParams.Add($"domain={domain}");
            }

            var queryString = string.Join("&", queryParams);

            return client.GetFromJsonAsAsyncEnumerable<BreachDetails>(
                    $"breachedaccount/{HttpUtility.UrlEncode(account)}?{queryString}",
                    SourceGeneratorContext.Default.BreachDetails,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return Internals.AsyncEnumerable.Empty<BreachDetails?>();
        }
    }    /// <inheritdoc cref="IPwnedBreachesClient.GetBreachHeadersForAccountAsync(string, CancellationToken)" />
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

            return Internals.AsyncEnumerable.Empty<BreachHeader?>();
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

            return Internals.AsyncEnumerable.Empty<string?>();
        }
    }

    /// <inheritdoc cref="IPwnedBreachesClient.GetLatestBreachAsync(CancellationToken)" />
    async Task<BreachDetails?> IPwnedBreachesClient.GetLatestBreachAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var breachDetails =
                await client.GetFromJsonAsync<BreachDetails>(
                        "latestbreach",
                        SourceGeneratorContext.Default.BreachDetails,
                        cancellationToken: cancellationToken
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
}