// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Represents the DNS TXT record returned when domain verification is started.
/// </summary>
public sealed record class DomainVerificationDnsToken
{
    /// <summary>
    /// Gets the value to publish in the domain's DNS TXT record.
    /// </summary>
    public string TxtRecordValue { get; init; } = null!;
}
