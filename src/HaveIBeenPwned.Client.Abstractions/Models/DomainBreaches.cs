// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Represents the breaches for a domain search.
/// The dictionary key is the email alias, and the value is an array of breach names.
/// See <a href="https://haveibeenpwned.com/API/v3#BreachedDomain"></a>
/// </summary>
public sealed class DomainBreaches : Dictionary<string, string[]>
{
}
