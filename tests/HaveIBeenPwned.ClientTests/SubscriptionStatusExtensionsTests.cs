// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client.Abstractions;
using HaveIBeenPwned.Client.Extensions;
using HaveIBeenPwned.Client.Options;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

public sealed class SubscriptionStatusExtensionsTests
{
    public static TheoryData<string, HibpSubscriptionLevel> CurrentPlans => new()
    {
        { "Core 1", HibpSubscriptionLevel.Core1 },
        { "Core 2", HibpSubscriptionLevel.Core2 },
        { "Core 3", HibpSubscriptionLevel.Core3 },
        { "Core 4", HibpSubscriptionLevel.Core4 },
        { "Core 5", HibpSubscriptionLevel.Core5 },
        { "Pro 1", HibpSubscriptionLevel.Pro1 },
        { "Pro 2", HibpSubscriptionLevel.Pro2 },
        { "Pro 3", HibpSubscriptionLevel.Pro3 },
        { "Pro 4", HibpSubscriptionLevel.Pro4 },
        { "Pro 5", HibpSubscriptionLevel.Pro5 },
        { "High RPM 4000", HibpSubscriptionLevel.HighRpm4000 },
        { "High RPM 8000", HibpSubscriptionLevel.HighRpm8000 },
        { "High RPM 12000", HibpSubscriptionLevel.HighRpm12000 },
        { "High RPM 16000", HibpSubscriptionLevel.HighRpm16000 },
        { "High RPM 24000", HibpSubscriptionLevel.HighRpm24000 }
    };

    [Theory]
    [MemberData(nameof(CurrentPlans))]
    public void GetSubscriptionLevelMapsCurrentPlans(
        string subscriptionName,
        HibpSubscriptionLevel expected)
    {
        var status = new SubscriptionStatus { SubscriptionName = subscriptionName };

        Assert.Equal(expected, status.GetSubscriptionLevel());
    }

    [Theory]
    [InlineData("Pwned 1", HibpSubscriptionLevel.Core1)]
    [InlineData("Pwned 2", HibpSubscriptionLevel.Core2)]
    [InlineData("Pwned 3", HibpSubscriptionLevel.Core3)]
    [InlineData("Pwned 4", HibpSubscriptionLevel.Core4)]
    public void GetSubscriptionLevelMapsLegacyPlanNames(
        string subscriptionName,
        HibpSubscriptionLevel expected)
    {
        var status = new SubscriptionStatus { SubscriptionName = subscriptionName };

        Assert.Equal(expected, status.GetSubscriptionLevel());
    }

    [Fact]
    public void LegacyEnumAliasesRetainNumericCompatibility()
    {
#pragma warning disable CS0618
        Assert.Equal(HibpSubscriptionLevel.Core1, HibpSubscriptionLevel.One);
        Assert.Equal(HibpSubscriptionLevel.Core2, HibpSubscriptionLevel.Two);
        Assert.Equal(HibpSubscriptionLevel.Core3, HibpSubscriptionLevel.Three);
        Assert.Equal(HibpSubscriptionLevel.Core4, HibpSubscriptionLevel.Four);
#pragma warning restore CS0618
    }

    [Fact]
    public void GetSubscriptionLevelRejectsUnknownPlan()
    {
        var status = new SubscriptionStatus { SubscriptionName = "Unknown 1" };

        Assert.Throws<ArgumentException>(() => status.GetSubscriptionLevel());
    }
}
