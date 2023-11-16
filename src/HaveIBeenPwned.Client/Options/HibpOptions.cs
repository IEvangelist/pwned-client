// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Options;

/// <summary>
/// The "Have I Been Pwned" API options object.
/// See <a href="https://haveibeenpwned.com/api"></a>
/// </summary>
public sealed class HibpOptions : IOptions<HibpOptions>
{
    static readonly string LibraryVersion =
        typeof(HibpOptions).Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()
            ?.Version ?? "1.0";

    internal static readonly string DefaultUserAgent =
        $".NET HIBP Client/{LibraryVersion}";

    private string? _userAgent = DefaultUserAgent;

    /// <summary>
    /// Gets or sets the API key, used to authorize HTTP calls to HIBP.
    /// See <a href="https://haveibeenpwned.com/api/v3#Authorisation"></a>
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP header value for <c>User-Agent</c>. Defaults to <c>.NET HIBP Client/1.0</c>.
    /// See <a href="https://haveibeenpwned.com/API/v3#UserAgent"></a>
    /// </summary>
    public string UserAgent
    {
        get => _userAgent ?? DefaultUserAgent;
        set => _userAgent = value ?? DefaultUserAgent;
    }

    /// <summary>
    /// Gets or sets the subscription level for the "Have I Been Pwned" API.
    /// The subscription level impacts rate limiting, and this setting can be 
    /// used to employ a rate-limit aware HTTP resilience strategy when using:
    /// <a href="https://www.nuget.org/packages/HaveIBeenPwned.Client.PollyExtensions"></a>.
    /// </summary>
    public HibpSubscriptionLevel? SubscriptionLevel { get; set;}

    /// <inheritdoc />
    HibpOptions IOptions<HibpOptions>.Value => this;

    internal void Deconstruct(out string apiKey, out string userAgent, out HibpSubscriptionLevel? subscriptionLevel) =>
        (apiKey, userAgent, subscriptionLevel) = (ApiKey, UserAgent, SubscriptionLevel);
}
