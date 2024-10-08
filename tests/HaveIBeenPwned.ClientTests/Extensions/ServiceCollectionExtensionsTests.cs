// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
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
    }
}
