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
    /// <returns>The same <paramref name="services"/> instance with other services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// If either the <paramref name="services"/> or <paramref name="namedConfigurationSection"/> are <c>null</c>.
    /// </exception>
    public static IServiceCollection AddPwnedServices(
        this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services), "The IServiceCollection cannot be null.");
        }

        if (namedConfigurationSection is null)
        {
            throw new ArgumentNullException(
                nameof(namedConfigurationSection), "The IConfiguration cannot be null.");
        }

        services.Configure<HibpOptions>(namedConfigurationSection);

        return AddPwnedServices(services);
    }

    /// <summary>
    /// Adds all of the necessary Pwned service functionality to
    /// the <paramref name="services"/> collection for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">The action used to configure options.</param>
    /// <returns>The same <paramref name="services"/> instance with other services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// If either the <paramref name="services"/> or <paramref name="configureOptions"/> are <c>null</c>.
    /// </exception>
    public static IServiceCollection AddPwnedServices(
        this IServiceCollection services,
        Action<HibpOptions> configureOptions)
    {
        if (services is null)
        {
            throw new ArgumentNullException(
                nameof(services), "The IServiceCollection cannot be null.");
        }

        if (configureOptions is null)
        {
            throw new ArgumentNullException(
                nameof(configureOptions), "The Action<HibpOptions> cannot be null.");
        }

        services.Configure(configureOptions);

        return AddPwnedServices(services);
    }

    static IServiceCollection AddPwnedServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddOptions<HibpOptions>();

        _ = AddPwnedHttpClient(
            services,
            HttpClientNames.HibpClient,
            HttpClientUrls.HibpApiUrl);

        _ = AddPwnedHttpClient(
            services,
            HttpClientNames.PasswordsClient,
            HttpClientUrls.PasswordsApiUrl,
            isPlainText: true);

        services.AddSingleton<IPwnedBreachesClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedPasswordsClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedPastesClient, DefaultPwnedClient>();
        services.AddSingleton<IPwnedClient, DefaultPwnedClient>();

        return services;
    }

    static IHttpClientBuilder AddPwnedHttpClient(
        IServiceCollection services,
        string httpClientName,
        string baseAddress,
        bool isPlainText = false) =>
        services.AddHttpClient(
            httpClientName,
            (serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<HibpOptions>>();
                var (apiKey, userAgent) = options?.Value
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
