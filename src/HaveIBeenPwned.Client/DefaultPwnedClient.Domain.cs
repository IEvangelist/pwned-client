// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedDomainClient
{
    /// <inheritdoc cref="IPwnedDomainClient.GetBreachedDomainAsync(string, CancellationToken)" />
    async Task<DomainBreaches?> IPwnedDomainClient.GetBreachedDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var domainBreaches =
                await client.GetFromJsonAsync<DomainBreaches>(
                        $"breacheddomain/{domain}",
                        SourceGeneratorContext.Default.DomainBreaches,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return domainBreaches;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedDomainClient.GetSubscribedDomainsAsync(CancellationToken)" />
    async Task<SubscribedDomain[]> IPwnedDomainClient.GetSubscribedDomainsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var subscribedDomains =
                await client.GetFromJsonAsync<SubscribedDomain[]>(
                        "subscribeddomains",
                        SourceGeneratorContext.Default.SubscribedDomainArray,
                        cancellationToken: cancellationToken
                    )
                    .ConfigureAwait(false);

            return subscribedDomains ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return [];
        }
    }

    /// <inheritdoc cref="IPwnedDomainClient.GetSubscribedDomainsAsAsyncEnumerable(CancellationToken)" />
    IAsyncEnumerable<SubscribedDomain?> IPwnedDomainClient.GetSubscribedDomainsAsAsyncEnumerable(
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            return client.GetFromJsonAsAsyncEnumerable<SubscribedDomain>(
                    "subscribeddomains",
                    SourceGeneratorContext.Default.SubscribedDomain,
                    cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return Internals.AsyncEnumerable.Empty<SubscribedDomain?>();
        }
    }
}
