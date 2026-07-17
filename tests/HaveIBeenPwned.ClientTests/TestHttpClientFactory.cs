// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using HaveIBeenPwned.Client.Http;

namespace HaveIBeenPwned.ClientTests;

internal sealed record class RecordedRequest(
    HttpMethod Method,
    Uri Uri,
    IReadOnlyDictionary<string, string[]> Headers,
    string? Content);

internal sealed class TestHttpClientFactory : IHttpClientFactory, IDisposable
{
    private readonly HttpClient _hibpClient;
    private readonly HttpClient _passwordsClient;

    internal TestHttpClientFactory(
        Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>> responder)
    {
        var handler = new RecordingHandler(Requests, responder);

        _hibpClient = new HttpClient(handler, disposeHandler: false)
        {
            BaseAddress = new Uri(HttpClientUrls.HibpApiUrl)
        };
        _passwordsClient = new HttpClient(handler, disposeHandler: false)
        {
            BaseAddress = new Uri(HttpClientUrls.PasswordsApiUrl)
        };
    }

    internal TestHttpClientFactory(Func<RecordedRequest, HttpResponseMessage> responder)
        : this((request, _) => Task.FromResult(responder(request)))
    {
    }

    internal List<RecordedRequest> Requests { get; } = [];

    internal static HttpResponseMessage Json(
        string json,
        HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    internal static HttpResponseMessage Text(
        string text,
        HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(statusCode)
        {
            Content = new StringContent(text, Encoding.UTF8, "text/plain")
        };

    HttpClient IHttpClientFactory.CreateClient(string name) => name switch
    {
        HttpClientNames.HibpClient => _hibpClient,
        HttpClientNames.PasswordsClient => _passwordsClient,
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
    };

    public void Dispose()
    {
        _hibpClient.Dispose();
        _passwordsClient.Dispose();
    }

    private sealed class RecordingHandler(
        List<RecordedRequest> requests,
        Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>> responder)
        : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var headers = request.Headers
                .Concat(request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(
                    static header => header.Key,
                    static header => header.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase);
            var content = request.Content is null
                ? null
                : await request.Content
                    .ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);
            var recordedRequest = new RecordedRequest(
                request.Method,
                request.RequestUri!,
                headers,
                content);

            requests.Add(recordedRequest);
            return await responder(recordedRequest, cancellationToken).ConfigureAwait(false);
        }
    }
}
