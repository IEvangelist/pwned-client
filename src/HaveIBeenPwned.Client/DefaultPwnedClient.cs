// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <inheritdoc cref="IPwnedClient" />
internal sealed partial class DefaultPwnedClient : IPwnedClient
{
    readonly IHttpClientFactory _httpClientFactory;
    readonly ILogger<DefaultPwnedClient> _logger;

    public DefaultPwnedClient(
        IHttpClientFactory httpClientFactory,
        ILogger<DefaultPwnedClient> logger) =>
        (_httpClientFactory, _logger) = (httpClientFactory, logger);
}
