// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Represents the stealer logs for an email domain.
/// The dictionary key is the email alias, and the value is an array of website domains.
/// See <a href="https://haveibeenpwned.com/API/v3#StealerLogsByEmailDomain"></a>
/// </summary>
public sealed class StealerLogsByEmailDomain : Dictionary<string, string[]>
{
}
