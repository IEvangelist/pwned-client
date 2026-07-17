// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Options;

/// <summary>
/// Represents a current or legacy Have I Been Pwned subscription plan.
/// </summary>
public enum HibpSubscriptionLevel
{
    /// <summary>Core 1.</summary>
    Core1 = 0,

    /// <summary>Core 2.</summary>
    Core2 = 1,

    /// <summary>Core 3.</summary>
    Core3 = 2,

    /// <summary>Core 4.</summary>
    Core4 = 3,

    /// <summary>Core 5.</summary>
    Core5 = 4,

    /// <summary>Pro 1.</summary>
    Pro1 = 5,

    /// <summary>Pro 2.</summary>
    Pro2 = 6,

    /// <summary>Pro 3.</summary>
    Pro3 = 7,

    /// <summary>Pro 4.</summary>
    Pro4 = 8,

    /// <summary>Pro 5.</summary>
    Pro5 = 9,

    /// <summary>High RPM 4000.</summary>
    HighRpm4000 = 10,

    /// <summary>High RPM 8000.</summary>
    HighRpm8000 = 11,

    /// <summary>High RPM 12000.</summary>
    HighRpm12000 = 12,

    /// <summary>High RPM 16000.</summary>
    HighRpm16000 = 13,

    /// <summary>High RPM 24000.</summary>
    HighRpm24000 = 14,

    /// <summary>The legacy Pwned 1 plan, now represented by Core 1.</summary>
    [Obsolete("Use Core1. The Pwned 1 plan name has been retired.")]
    One = Core1,

    /// <summary>The legacy Pwned 2 plan, now represented by Core 2.</summary>
    [Obsolete("Use Core2. The Pwned 2 plan name has been retired.")]
    Two = Core2,

    /// <summary>The legacy Pwned 3 plan, now represented by Core 3.</summary>
    [Obsolete("Use Core3. The Pwned 3 plan name has been retired.")]
    Three = Core3,

    /// <summary>The legacy Pwned 4 plan, now represented by Core 4.</summary>
    [Obsolete("Use Core4. The Pwned 4 plan name has been retired.")]
    Four = Core4
}
