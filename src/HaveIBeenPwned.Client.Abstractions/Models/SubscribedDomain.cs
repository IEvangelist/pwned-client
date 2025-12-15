// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Represents a subscribed domain in the HIBP system.
/// See <a href="https://haveibeenpwned.com/API/v3#SubscribedDomains"></a>
/// </summary>
public sealed record class SubscribedDomain
{
    /// <summary>
    /// The full domain name that has been successfully verified.
    /// </summary>
    public string DomainName { get; init; } = null!;

    /// <summary>
    /// The total number of breached email addresses found on the domain at last search (will be null if no searches yet performed).
    /// </summary>
    public int? PwnCount { get; init; }

    /// <summary>
    /// The number of breached email addresses found on the domain at last search, excluding any breaches flagged as a spam list (will be null if no searches yet performed).
    /// </summary>
    public int? PwnCountExcludingSpamLists { get; init; }

    /// <summary>
    /// The total number of breached email addresses found on the domain when the current subscription was taken out (will be null if no searches yet performed). This number ensures the domain remains searchable throughout the subscription period even if the volume of breached accounts grows beyond the subscription's scope.
    /// </summary>
    public int? PwnCountExcludingSpamListsAtLastSubscriptionRenewal { get; init; }

    /// <summary>
    /// The date and time the current subscription ends in ISO 8601 format. The PwnCountExcludingSpamListsAtLastSubscriptionRenewal value is locked in until this time (will be null if there have been no subscriptions).
    /// </summary>
    public DateTime? NextSubscriptionRenewal { get; init; }
}
