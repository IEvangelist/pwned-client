// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Factories;

/// <summary>
/// A "pwned" client factory. Call <see cref="FromApiKey(string, ILoggerFactory?)"/> to
/// get a hydrated <see cref="IPwnedClient"/> instance.
/// </summary>
/// <remarks>
/// This is static, and not intended for usage in conjunction with dependency injection
/// (DI). In other words, if you don't want to use DI you can use this instead.
/// </remarks>
public static class PwnedClientFactory
{
    /// <summary>
    /// Creates and returns an <see cref="IPwnedClient"/> implementation from the given
    /// <paramref name="apiKey"/> value. Consider the following example usage:
    /// <code language="csharp">
    /// IPwnedClient client = PwnedClientFactory.FromApiKey(configuration["ApiKey"]);
    /// // TODO: use client...
    /// </code>
    /// </summary>
    /// <param name="apiKey">
    /// The API key, used to authorize HTTP calls to HIBP.
    /// See <a href="https://haveibeenpwned.com/api/v3#Authorisation"></a>
    /// </param>
    /// <param name="loggerFactory">
    /// An optional <see cref="ILoggerFactory"/> to create the
    /// <see cref="ILogger"/> used within the <see cref="PwnedClient"/> instance
    /// </param>
    /// <remarks>
    /// This is static, and not intended for usage in conjunction with dependency injection
    /// (DI). In other words, if you don't want to use DI you can use this instead.
    /// </remarks>
    /// <returns>
    /// An <see cref="IPwnedClient"/> implementation from the given
    /// <paramref name="apiKey"/> value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the given <paramref name="apiKey"/> value is <see langword="null" />, this exception is thrown.
    /// </exception>
    internal static IPwnedClient FromApiKey(string apiKey, ILoggerFactory? loggerFactory = default)
    {
        var httpClientFactory = InternalHttpClientFactory.Create(apiKey);

        loggerFactory ??= NullLoggerFactory.Instance;

        var logger = loggerFactory.CreateLogger<DefaultPwnedClient>();

        return new DefaultPwnedClient(httpClientFactory, logger);
    }
}
