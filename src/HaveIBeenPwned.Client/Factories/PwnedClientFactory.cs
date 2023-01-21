// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Loggers;

namespace HaveIBeenPwned.Client.Factories;

/// <summary>
/// A "pwned" client factory.
/// </summary>
/// <remarks>
/// This is static, and not intended for usage in conjunction with dependency injection
/// (DI). In other words, if you don't want to use DI you can use this instead.
/// </remarks>
internal static class PwnedClientFactory
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
    /// <remarks>
    /// This is static, and not intended for usage in conjunction with dependency injection
    /// (DI). In other words, if you don't want to use DI you can use this instead.
    /// </remarks>
    /// <returns>
    /// An <see cref="IPwnedClient"/> implementation from the given
    /// <paramref name="apiKey"/> value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// If the given <paramref name="apiKey"/> value is <c>null</c>, this exception is thrown.
    /// </exception>
    internal static IPwnedClient FromApiKey(string apiKey) =>
        new DefaultPwnedClient(
            HttpClientFactory.Create(apiKey),
            SimpleConsoleLogger<DefaultPwnedClient>.Instance);
}
