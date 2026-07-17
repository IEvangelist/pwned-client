// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Http;
using HaveIBeenPwned.Client.Options;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HaveIBeenPwned.ClientTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPwnedServices_Throws_When_ServiceCollectionIsNull() => Assert.Throws<ArgumentNullException>(
            "services",
            () => ((IServiceCollection)null!).AddPwnedServices(_ => { }));

    [Fact]
    public void AddPwnedServices_Throws_When_ConfigureOptionsIsNull() => Assert.Throws<ArgumentNullException>(
            "configureOptions",
            () => new ServiceCollection().AddPwnedServices(
                (null as Action<HibpOptions>)!));

    [Fact]
    public void AddPwnedServices_AddsDefaultImplementations_When_Valid()
    {
        var services = new ServiceCollection()
            .AddPwnedServices(options => { })
            .BuildServiceProvider();

        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedBreachesClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPasswordsClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPastesClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedDomainClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedStealerLogsClient>().GetType());
    }

    [Fact]
    public void AddPwnedServices_ConfiguresAuthenticationOnlyForHibpClient()
    {
        using var services = new ServiceCollection()
            .AddPwnedServices(options =>
            {
                options.ApiKey = "test-key";
                options.UserAgent = "test-client/1.0";
            })
            .BuildServiceProvider();
        var factory = services.GetRequiredService<IHttpClientFactory>();

        using var hibpClient = factory.CreateClient(HttpClientNames.HibpClient);
        using var passwordsClient = factory.CreateClient(HttpClientNames.PasswordsClient);

        Assert.Equal(
            ["test-key"],
            hibpClient.DefaultRequestHeaders.GetValues(HttpHeaderNames.HibpApiKey));
        Assert.Equal("test-client/1.0", hibpClient.DefaultRequestHeaders.UserAgent.ToString());
        Assert.False(
            passwordsClient.DefaultRequestHeaders.Contains(HttpHeaderNames.HibpApiKey));
        Assert.Equal(
            "test-client/1.0",
            passwordsClient.DefaultRequestHeaders.UserAgent.ToString());
    }
}
