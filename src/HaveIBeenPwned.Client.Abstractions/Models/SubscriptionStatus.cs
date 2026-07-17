// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Represents the current status of a subscription.
/// </summary>
public sealed record class SubscriptionStatus
{
    /// <summary>
    /// Gets the name representing the subscription being either
    /// the current plan and level, for example "Pro 2".
    /// </summary>
    public string SubscriptionName { get; init; } = "";

    /// <summary>
    /// Gets a human readable sentence explaining the scope of the subscription.
    /// </summary>
    public string Description { get; init; } = "";

    /// <summary>
    /// Gets the date and time the current subscription ends in ISO 8601 format.
    /// </summary>
    public DateTime SubscribedUntil { get; init; }

    /// <summary>
    /// Gets the rate limit in requests per minute. This applies to the rate
    /// the breach search by email address API can be requested.
    /// </summary>
    public int Rpm { get; init; }

    /// <summary>
    /// Gets the size of the largest domain the subscription can search. This is
    /// expressed in the total number of breached accounts on the domain,
    /// excluding those that appear solely in spam list.
    /// </summary>
    public int DomainSearchMaxBreachedAccounts { get; init; }

    /// <summary>
    /// Gets the maximum number of breached domains the subscription can add.
    /// A null value indicates that there is no domain limit.
    /// </summary>
    public int? MaxBreachedDomains { get; init; }

    /// <summary>
    /// Indicates if the subscription includes access to the stealer logs APIs.
    /// </summary>
    public bool IncludesStealerLogs { get; init; }

    /// <summary>
    /// Indicates if the subscription includes the bulk domain verification APIs.
    /// </summary>
    public bool IncludesBulkDomainAdd { get; init; }

    /// <summary>
    /// Indicates if verified apex domains automatically verify their subdomains.
    /// </summary>
    public bool IncludesAutoSubdomainVerification { get; init; }

    /// <summary>
    /// Indicates if customer domains can be added to the subscription.
    /// </summary>
    public bool IncludesCustomerDomains { get; init; }

    /// <summary>
    /// Indicates if the subscription includes k-anonymity email address searches.
    /// </summary>
    public bool IncludesKAnon { get; init; }
}
