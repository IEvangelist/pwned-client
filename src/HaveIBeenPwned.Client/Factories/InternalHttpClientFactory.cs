// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Factories;

internal sealed class InternalHttpClientFactory : IHttpClientFactory
{
    private static readonly SocketsHttpHandler s_handler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15)
    };

    private readonly HttpClient _hibpClient;
    private readonly HttpClient _passwordsClient;

    private InternalHttpClientFactory(string? apiKey)
    {
        _hibpClient = new HttpClient(s_handler, disposeHandler: false)
        {
            BaseAddress = new Uri(HttpClientUrls.HibpApiUrl)
        };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _hibpClient.DefaultRequestHeaders.Add(HttpHeaderNames.HibpApiKey, apiKey);
        }
        _hibpClient.DefaultRequestHeaders.UserAgent.ParseAdd(HibpOptions.DefaultUserAgent);

        _passwordsClient = new HttpClient(s_handler, disposeHandler: false)
        {
            BaseAddress = new Uri(HttpClientUrls.PasswordsApiUrl)
        };
        _passwordsClient.DefaultRequestHeaders.UserAgent.ParseAdd(HibpOptions.DefaultUserAgent);
        _passwordsClient.DefaultRequestHeaders.Accept.Add(
            new(MediaTypeNames.Text.Plain));
    }

    internal static InternalHttpClientFactory Create(string? apiKey) => new(apiKey);

    HttpClient IHttpClientFactory.CreateClient(string name) => name switch
    {
        HttpClientNames.HibpClient => _hibpClient,
        HttpClientNames.PasswordsClient => _passwordsClient,
        _ => throw new NotImplementedException()
    };
}
