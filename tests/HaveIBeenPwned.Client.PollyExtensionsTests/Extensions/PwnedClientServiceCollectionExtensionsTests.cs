// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Xunit;

namespace HaveIBeenPwned.Client.PollyExtensionsTests.Extensions;

public class PwnedClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPwnedServicesThrowsWhenServiceCollectionIsNull() => Assert.Throws<ArgumentNullException>(
        "services",
        () => ((IServiceCollection)null!).AddPwnedServices(_ => { }));

    [Fact]
    public void AddPwnedServicesThrowsWhenConfigureOptionsIsNull() => Assert.Throws<ArgumentNullException>(
            "configureOptions",
            () => new ServiceCollection().AddPwnedServices(
                (null as Action<HibpOptions>)!));

    [Fact]
    public void AddPwnedServicesThrowsWhenConfigureRetryPolicyIsNull() => Assert.Throws<ArgumentNullException>(
            "configureResilienceOptions",
            () => new ServiceCollection().AddPwnedServices(
                options => { },
                (null as Action<HttpStandardResilienceOptions>)!));

    [Fact]
    public void AddPwnedServicesAddsDefaultImplementationsWhenValid()
    {
        static void TestPolicyFactory(HttpStandardResilienceOptions _) { }

        var services = new ServiceCollection()
            .AddPwnedServices(options => { }, TestPolicyFactory)
            .BuildServiceProvider();

        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedBreachesClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPasswordsClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPastesClient>().GetType());
    }
}
