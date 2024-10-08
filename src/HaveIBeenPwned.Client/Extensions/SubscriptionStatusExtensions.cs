// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Extensions;

/// <summary>
/// Extensions methods for the <see cref="SubscriptionStatus"/> class.
/// </summary>
public static class SubscriptionStatusExtensions
{
    /// <summary>
    /// Gets the <see cref="HibpSubscriptionLevel"/> from the given <paramref name="subscriptionStatus"/>.
    /// </summary>
    /// <param name="subscriptionStatus">The subscription status instance to evaluate.</param>
    /// <returns>The corresponding subscription level.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the <see cref="SubscriptionStatus.SubscriptionName"/>
    /// doesn't map to a level.
    /// </exception>
    public static HibpSubscriptionLevel GetSubscriptionLevel(
        this SubscriptionStatus subscriptionStatus) => subscriptionStatus.SubscriptionName switch
        {
            "Pwned 1" => HibpSubscriptionLevel.One,
            "Pwned 2" => HibpSubscriptionLevel.Two,
            "Pwned 3" => HibpSubscriptionLevel.Three,
            "Pwned 4" => HibpSubscriptionLevel.Four,

            _ => throw new ArgumentException(
                $"Unknown subscription level: {subscriptionStatus.SubscriptionName}.")
        };
}
