// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using HaveIBeenPwned.Client.Extensions;

namespace HaveIBeenPwned.ClientTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddPwnedServices_Throws_When_ServiceCollectionIsNull() =>
            Assert.Throws<ArgumentNullException>(
                "services",
                () => ((IServiceCollection)null).AddPwnedServices(_ => { }));

        [Fact]
        public void AddPwnedServices_Throws_When_ConfigureOptionsIsNull() =>
            Assert.Throws<ArgumentNullException>(
                "configureOptions",
                () => new ServiceCollection().AddPwnedServices(null));
    }
}
