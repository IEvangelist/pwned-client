// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// See <a href="https://haveibeenpwned.com/api/v3#BreachModel"></a>
/// </summary>
public class BreachHeader
{
    /// <summary>
    /// A Pascal-cased name representing the breach which is unique across all other breaches.
    /// This value never changes and may be used to name dependent assets (such as images)
    /// but should not be shown directly to end users (see the "Title" attribute instead).
    /// </summary>
    public string Name { get; set; } = null!;
}
