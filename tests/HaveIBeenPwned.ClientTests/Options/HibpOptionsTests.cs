// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using HaveIBeenPwned.Client.Options;
using Xunit;

using static Microsoft.Extensions.Options.Options;

namespace HaveIBeenPwned.ClientTests.Options
{
    public class HibpOptionsTests
    {
        [Fact]
        public void HibpOptions_Returns_CorrectlyFormattedUserAgent()
        {
            var options = Create<HibpOptions>(new());
            var version = typeof(HibpOptions).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

            Assert.Equal($".NET HIBP Client/{version}", options.Value.UserAgent);
        }

        [Fact]
        public void HibpOptions_DeconstructsProperly()
        {
            var options = Create<HibpOptions>(new() { ApiKey = "Test", UserAgent = "Fake" });
            var (apiKey, userAgent) = options.Value;

            Assert.Equal("Test", apiKey);
            Assert.Equal("Fake", userAgent);
        }
    }
}
