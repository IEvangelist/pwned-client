// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Serialization;

internal sealed record class BreachedAccountHashRange
{
    public string HashSuffix { get; init; } = null!;

    public string[] Websites { get; init; } = [];
}

internal sealed record class DomainVerificationRequest
{
    [JsonPropertyName("DomainName")]
    public string DomainName { get; init; } = null!;
}

internal sealed record class DomainVerificationEmailRequest
{
    [JsonPropertyName("DomainName")]
    public string DomainName { get; init; } = null!;

    [JsonPropertyName("EmailAlias")]
    public string EmailAlias { get; init; } = null!;
}
