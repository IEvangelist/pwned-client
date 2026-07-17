// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <inheritdoc cref="IPwnedClient" />
internal sealed partial class DefaultPwnedClient(
    IHttpClientFactory httpClientFactory,
    ILogger<DefaultPwnedClient> logger) : IPwnedClient
{
    async Task<SubscriptionStatus?> IPwnedClient.GetSubscriptionStatusAsync(
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                "subscription/status",
                SourceGeneratorContext.Default.SubscriptionStatus,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false);
}
