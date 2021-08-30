// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Extensions;
using HaveIBeenPwned.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HaveIBeenPwned.ClientTests
{
    public class DefaultPwnedClientTests
    {
        [Fact]
        public async Task GetBreachAsync_Throws_WhenBreachNameIsNull()
        {
            IPwnedBreachesClient sut = new DefaultPwnedClient(
                NullHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            await Assert.ThrowsAsync<ArgumentException>(
                () => sut.GetBreachAsync(breachName: null));
        }

        [Fact]
        public async Task GetBreachesForAccountAsync_Throws_WhenAccountIsNull()
        {
            IPwnedBreachesClient sut = new DefaultPwnedClient(
                NullHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            await Assert.ThrowsAsync<ArgumentException>(
                () => sut.GetBreachesForAccountAsync(account: null));
        }

        [Fact]
        public async Task GetBreachesForAccountAsync_ReturnsEmptyArray_WhenErrant()
        {
            IPwnedBreachesClient sut = new DefaultPwnedClient(
                ThrowingHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            var actual = await sut.GetBreachesForAccountAsync(
                "account-exists@hibp-integration-tests.com");
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetBreachHeadersForAccountAsync_Throws_WhenAccountIsNull()
        {
            IPwnedBreachesClient sut = new DefaultPwnedClient(
                NullHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await sut.GetBreachHeadersForAccountAsync(account: null));
        }

        [Fact]
        public async Task GetBreachHeadersForAccountAsync_ReturnsEmptyArray_WhenErrant()
        {
            IPwnedBreachesClient sut = new DefaultPwnedClient(
                ThrowingHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            var actual = await sut.GetBreachHeadersForAccountAsync(
                "account-exists@hibp-integration-tests.com");
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetPastesAsync_Throws_WhenAccountIsNull()
        {
            IPwnedPastesClient sut = new DefaultPwnedClient(
                NullHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await sut.GetPastesAsync(account: null));
        }

        [Fact]
        public async Task GetPastesAsync_ReturnsEmptyArray_WhenErrant()
        {
            IPwnedPastesClient sut = new DefaultPwnedClient(
                ThrowingHttpClientFactory.Instance,
                NullLoggerFactory.Instance.CreateLogger<DefaultPwnedClient>());

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await sut.GetPastesAsync(account: null));
        }

        public static IEnumerable<object[]> ParsePasswordRangeResponseTextInput
        {
            get
            {
                yield return new object[]
                {
                    new PwnedPassword("f@k3PA55w0d!")
                    {
                        HashedPassword = "a841ab792c6b438c71a97e05e6197ceba54f8e96",
                        PwnedCount = 77,
                        IsPwned = true
                    },
                    "f@k3PA55w0d!",
                    @"0018A45C4D1DEF81644B54AB7F969B88D65:1
00D4F6E8FA6EECAD2A3AA415EEC418D38EC:2
011053FD0102E94D6AE2F8B83D76FAF94F6:1
012A7CA357541F0AC487871FEEC1891C49C:2
b792c6b438c71a97e05e6197ceba54f8e96:77
0136E006E24E7D152139815FB0FC6A50B15:2"
                };

                yield return new object[]
                {
                    new PwnedPassword("Does this even work?")
                    {
                        HashedPassword = "5ec3e89686893f2ba76bdf1bcf1070568823e400"
                    },
                    "Does this even work?",
                    @"0018A45C4D1DEF81644B54AB7F969B88D65:8
00D4F6E8FA6EECAD2A3AA415EEC418D38EC:9
PICKLES:CHIPS
012A7CA357541F0AC487871FEEC1891C49C:2
09D4F6E8FA6EECAD2A3AA415EEC418D38EC:903

011053FD0102E94D6AE2F8B83D76FAF94F6:34634
PICKLES:999999999999999999999999999999
0792c6b438c71a97e05e6197ceba54f8e96:234
0136E006E24E7D152139815FB0FC6A50B15:1"
                };

                yield return new object[]
                {
                    new PwnedPassword("f@k3PA55w0d!")
                    {
                        HashedPassword = "a841ab792c6b438c71a97e05e6197ceba54f8e96",
                        PwnedCount = 4001,
                        IsPwned = true
                    },
                    "f@k3PA55w0d!",
                    @"0018A45C4D1DEF81644B54AB7F969B88D65:1
00D4F6E8FA6EECAD2A3AA415EEC418D38EC:bit
011053FD0102E94D6AE2F8B83D76FAF94F6:null
012A7CA357541F0AC487871FEEC1891C49C:2
b792c6b438c71a97e05e6197ceba54f8e96:4001
::::::
(DateTime):2"
                };

                yield return new object[]
                {
                    new PwnedPassword("f@k3PA55w0d!")
                    {
                        HashedPassword = "a841ab792c6b438c71a97e05e6197ceba54f8e96"
                    },
                    "f@k3PA55w0d!",
                    null
                };
            }
        }

        [
            Theory,
            MemberData(nameof(ParsePasswordRangeResponseTextInput))
        ]
        public void ParsePasswordRangeResponseText_CorrectlyHandlesText(
            PwnedPassword expected, string plainTextPassword, string passwordRangeResponseText)
        {
            PwnedPassword actual = new(plainTextPassword);
            var passwordHash = plainTextPassword.ToSha1Hash();

            actual = DefaultPwnedClient.ParsePasswordRangeResponseText(
                actual, passwordRangeResponseText, passwordHash);
            Assert.Equal(expected, actual);
        }
    }

    public class NullHttpClientFactory : IHttpClientFactory
    {
        public static readonly IHttpClientFactory Instance = new NullHttpClientFactory();

        HttpClient IHttpClientFactory.CreateClient(string name) => default;
    }

    public class ThrowingHttpClientFactory : IHttpClientFactory
    {
        public static readonly IHttpClientFactory Instance = new NullHttpClientFactory();

        HttpClient IHttpClientFactory.CreateClient(string name) => throw default!;
    }
}
