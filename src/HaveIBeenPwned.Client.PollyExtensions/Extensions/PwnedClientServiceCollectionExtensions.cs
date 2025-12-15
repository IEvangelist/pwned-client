// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up pwned client related services in an <see cref="IServiceCollection" />.
/// </summary>
public static class PwnedClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds all of the necessary Pwned service functionality to
    /// the <paramref name="services"/> collection for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="namedConfigurationSection">The name configuration section to bind options from.</param>
    /// <param name="configureResilienceOptions">The retry policy configuration function, when provided adds transient HTTP error policy.</param>
    /// <returns>The same <paramref name="services"/> instance with other services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// If either the <paramref name="services"/> or <paramref name="namedConfigurationSection"/> are <see langword="null" />.
    /// </exception>
    public static IServiceCollection AddPwnedServices(
        this IServiceCollection services,
        IConfiguration namedConfigurationSection,
        Action<HttpStandardResilienceOptions> configureResilienceOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(namedConfigurationSection);
        ArgumentNullException.ThrowIfNull(configureResilienceOptions);

        services.Configure<HibpOptions>(namedConfigurationSection);

        return AddPwnedServices(services, configureResilienceOptions);
    }

    /// <summary>
    /// Adds all of the necessary Pwned service functionality to
    /// the <paramref name="services"/> collection for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">The action used to configure options.</param>
    /// <param name="configureResilienceOptions">The retry policy configuration function, when provided adds transient HTTP error policy.</param>
    /// <returns>The same <paramref name="services"/> instance with other services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// If either the <paramref name="services"/> or <paramref name="configureOptions"/> are <see langword="null" />.
    /// </exception>
    public static IServiceCollection AddPwnedServices(
        this IServiceCollection services,
        Action<HibpOptions> configureOptions,
        Action<HttpStandardResilienceOptions> configureResilienceOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        ArgumentNullException.ThrowIfNull(configureResilienceOptions);

        services.Configure(configureOptions);

        return AddPwnedServices(services, configureResilienceOptions);
    }

    static IServiceCollection AddPwnedServices(
        IServiceCollection services,
        Action<HttpStandardResilienceOptions>? configureResilienceOptions)
    {
        services.AddLogging();
        services.AddOptions<HibpOptions>();

        var builder = AddPwnedHttpClient(
            services,
            HttpClientNames.HibpClient,
            HttpClientUrls.HibpApiUrl);

        builder.ConfigureHttpResilience(configureResilienceOptions);

        builder = AddPwnedHttpClient(
            services,
            HttpClientNames.PasswordsClient,
            HttpClientUrls.PasswordsApiUrl,
            isPlainText: true);

        builder.ConfigureHttpResilience(configureResilienceOptions);

        services.AddSingleton<IPwnedBreachesClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedPasswordsClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedPastesClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedDomainClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedStealerLogsClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedClient, DefaultPwnedClient>();

        return services;
    }

    static IHttpClientBuilder AddPwnedHttpClient(
        IServiceCollection services,
        string httpClientName,
        string baseAddress,
        bool isPlainText = false) => services.AddHttpClient(
            httpClientName,
            (serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<HibpOptions>>();
                var (apiKey, userAgent, _) = options?.Value
                    ?? throw new InvalidOperationException(
                        "The 'Have I Been Pwned' options object cannot be null.");

                client.BaseAddress = new(baseAddress);
                client.DefaultRequestHeaders.Add("hibp-api-key", apiKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

                if (isPlainText)
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new(MediaTypeNames.Text.Plain));
                }
            });
}
