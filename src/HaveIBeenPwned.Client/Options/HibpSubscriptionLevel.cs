// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Options;

/// <summary>
/// Represents the subscription level for the "Have I Been Pwned" API.
/// For more information on how a subscription level impacts various rate limiting,
/// see <a href="https://haveibeenpwned.com/API/v3#RateLimiting"></a> and
/// <a href="https://haveibeenpwned.com/API/Key"></a>.
/// </summary>
public enum HibpSubscriptionLevel
{
    /// <summary>
    /// Represents the subscription level of one. As documented in 
    /// <a href="https://haveibeenpwned.com/API/Key"></a>, this subscription level allows
    /// 10PRM and $3.95 per month (or $39.50 annually).
    /// A rate limited key allowing 10 email address searches per minute, and searches of 
    /// domains with up to 25 breached email addresses each.
    /// </summary>
    One,

    /// <summary>
    /// Represents the subscription level of two. As documented in 
    /// <a href="https://haveibeenpwned.com/API/Key"></a>, this subscription level allows
    /// 50PRM and $16.95 per month (or $169.50 annually).
    /// A rate limited key allowing 50 email address searches per minute, and searches of 
    /// domains with up to 100 breached email addresses each.
    /// </summary>
    Two,

    /// <summary>
    /// Represents the subscription level of three. As documented in 
    /// <a href="https://haveibeenpwned.com/API/Key"></a>, this subscription level allows
    /// 100PRM and $28.50 per month (or $285 annually).
    /// A rate limited key allowing 100 email address searches per minute, and searches of 
    /// domains with up to 500 breached email addresses each .
    /// </summary>
    Three,

    /// <summary>
    /// Represents the subscription level of four. As documented in 
    /// <a href="https://haveibeenpwned.com/API/Key"></a>, this subscription level allows
    /// 500PRM and $115 per month (or $1,150 annually).
    /// A rate limited key allowing 500 email address searches per minute, and domain searches with unlimited breached email addresses each.
    /// </summary>
    Four
}
