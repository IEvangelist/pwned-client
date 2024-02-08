// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <inheritdoc cref="IPwnedClient" />
internal sealed partial class DefaultPwnedClient(
    IHttpClientFactory httpClientFactory,
    ILogger<DefaultPwnedClient> logger) : IPwnedClient
{
}
