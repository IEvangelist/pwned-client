// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <inheritdoc cref="IPwnedClient" />
internal sealed partial class DefaultPwnedClient(
    IHttpClientFactory httpClientFactory,
    ILogger<DefaultPwnedClient> logger) : IPwnedClient
{
    /// <inheritdoc cref="IPwnedClient.GetSubscriptionStatusAsync(CancellationToken)" />
    async Task<SubscriptionStatus?> IPwnedClient.GetSubscriptionStatusAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HibpClient);

            var subscriptionStatus =
                await client.GetFromJsonAsync<SubscriptionStatus>(
                        "subscription/status",
                        SourceGeneratorContext.Default.SubscriptionStatus,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

            return subscriptionStatus;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExceptionMessage}", ex.Message);

            return null!;
        }
    }
}
