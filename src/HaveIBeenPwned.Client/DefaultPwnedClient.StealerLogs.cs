// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedStealerLogsClient
{
    /// <inheritdoc cref="IPwnedStealerLogsClient.GetStealerLogsByEmailAsync(string, CancellationToken)" />
    async Task<string[]?> IPwnedStealerLogsClient.GetStealerLogsByEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var domains =
                await client.GetFromJsonAsync<string[]>(
                        $"stealerlogsbyemail/{HttpUtility.UrlEncode(emailAddress)}",
                        SourceGeneratorContext.Default.StringArray,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return domains;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedStealerLogsClient.GetStealerLogsByWebsiteDomainAsync(string, CancellationToken)" />
    async Task<string[]?> IPwnedStealerLogsClient.GetStealerLogsByWebsiteDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var emailAddresses =
                await client.GetFromJsonAsync<string[]>(
                        $"stealerlogsbywebsitedomain/{domain}",
                        SourceGeneratorContext.Default.StringArray,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return emailAddresses;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }

    /// <inheritdoc cref="IPwnedStealerLogsClient.GetStealerLogsByEmailDomainAsync(string, CancellationToken)" />
    async Task<StealerLogsByEmailDomain?> IPwnedStealerLogsClient.GetStealerLogsByEmailDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var stealerLogs =
                await client.GetFromJsonAsync<StealerLogsByEmailDomain>(
                        $"stealerlogsbyemaildomain/{domain}",
                        SourceGeneratorContext.Default.StealerLogsByEmailDomain,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return stealerLogs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }
}
