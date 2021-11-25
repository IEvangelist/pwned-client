// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Xunit;
using HaveIBeenPwned.Client.Options;

namespace HaveIBeenPwned.ClientTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPwnedServices_Throws_When_ServiceCollectionIsNull() =>
        Assert.Throws<ArgumentNullException>(
            "services",
            () => ((IServiceCollection)null!).AddPwnedServices(_ => { }));

    [Fact]
    public void AddPwnedServices_Throws_When_ConfigureOptionsIsNull() =>
        Assert.Throws<ArgumentNullException>(
            "configureOptions",
            () => new ServiceCollection().AddPwnedServices(
                (null as Action<HibpOptions>)!));
}
