// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Http;

static class HttpClientNames
{
    /// <summary>
    /// HTTP client name corresponding to the <see cref="HttpClientUrls.HibpApiUrl"/>.
    /// </summary>
    internal const string HibpClient = nameof(HibpClient);

    /// <summary>
    /// HTTP client name corresponding to the <see cref="HttpClientUrls.PasswordsApiUrl"/>.
    /// </summary>
    internal const string PasswordsClient = nameof(PasswordsClient);
}
