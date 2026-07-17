// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Extensions;

/// <summary>
/// Extension methods for the <see cref="SubscriptionStatus"/> class.
/// </summary>
public static class SubscriptionStatusExtensions
{
    /// <summary>
    /// Gets the <see cref="HibpSubscriptionLevel"/> represented by a subscription status.
    /// </summary>
    /// <param name="subscriptionStatus">The subscription status instance to evaluate.</param>
    /// <returns>The corresponding subscription level.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="subscriptionStatus"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the subscription name is not recognized.
    /// </exception>
    public static HibpSubscriptionLevel GetSubscriptionLevel(
        this SubscriptionStatus subscriptionStatus)
    {
        ArgumentNullException.ThrowIfNull(subscriptionStatus);

        return subscriptionStatus.SubscriptionName.Trim() switch
        {
            "Core 1" or "Pwned 1" => HibpSubscriptionLevel.Core1,
            "Core 2" or "Pwned 2" => HibpSubscriptionLevel.Core2,
            "Core 3" or "Pwned 3" => HibpSubscriptionLevel.Core3,
            "Core 4" or "Pwned 4" => HibpSubscriptionLevel.Core4,
            "Core 5" => HibpSubscriptionLevel.Core5,
            "Pro 1" => HibpSubscriptionLevel.Pro1,
            "Pro 2" => HibpSubscriptionLevel.Pro2,
            "Pro 3" => HibpSubscriptionLevel.Pro3,
            "Pro 4" => HibpSubscriptionLevel.Pro4,
            "Pro 5" => HibpSubscriptionLevel.Pro5,
            "High RPM 4000" => HibpSubscriptionLevel.HighRpm4000,
            "High RPM 8000" => HibpSubscriptionLevel.HighRpm8000,
            "High RPM 12000" => HibpSubscriptionLevel.HighRpm12000,
            "High RPM 16000" => HibpSubscriptionLevel.HighRpm16000,
            "High RPM 24000" => HibpSubscriptionLevel.HighRpm24000,
            _ => throw new ArgumentException(
                $"Unknown subscription level: {subscriptionStatus.SubscriptionName}.",
                nameof(subscriptionStatus))
        };
    }
}
