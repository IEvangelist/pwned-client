// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Factories;

internal sealed class InternalHttpClientFactory : IHttpClientFactory
{
    private static string? _apiKey;

    private static readonly Lazy<HttpClient> _hibpClient = new(() =>
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(HttpClientUrls.HibpApiUrl)
        };
        client.DefaultRequestHeaders.Add("hibp-api-key", _apiKey);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HibpOptions.DefaultUserAgent);

        return client;
    });

    private static readonly Lazy<HttpClient> _passwordsClient = new(() =>
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(HttpClientUrls.PasswordsApiUrl)
        };
        client.DefaultRequestHeaders.Add("hibp-api-key", _apiKey);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(HibpOptions.DefaultUserAgent);
        client.DefaultRequestHeaders.Accept.Add(
            new(MediaTypeNames.Text.Plain));

        return client;
    });

    internal static InternalHttpClientFactory Create(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(apiKey);

        return new();
    }

    private InternalHttpClientFactory() { }

    HttpClient IHttpClientFactory.CreateClient(string name) =>
        name switch
        {
            HttpClientNames.HibpClient => _hibpClient.Value,
            HttpClientNames.PasswordsClient => _passwordsClient.Value,
            _ => throw new NotImplementedException()
        };
}
