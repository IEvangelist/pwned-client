// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedStealerLogsClient
{
    async Task<string[]?> IPwnedStealerLogsClient.GetStealerLogsByEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);

        return await GetJsonAsync(
                HibpClient,
                $"stealerlogsbyemail/{EscapePathSegment(emailAddress)}",
                SourceGeneratorContext.Default.StringArray,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task<string[]?> IPwnedStealerLogsClient.GetStealerLogsByWebsiteDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return await GetJsonAsync(
                HibpClient,
                $"stealerlogsbywebsitedomain/{EscapePathSegment(domain)}",
                SourceGeneratorContext.Default.StringArray,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task<StealerLogsByEmailDomain?> IPwnedStealerLogsClient.GetStealerLogsByEmailDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return await GetJsonAsync(
                HibpClient,
                $"stealerlogsbyemaildomain/{EscapePathSegment(domain)}",
                SourceGeneratorContext.Default.StealerLogsByEmailDomain,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
