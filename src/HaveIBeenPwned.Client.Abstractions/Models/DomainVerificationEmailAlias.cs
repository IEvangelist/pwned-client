// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// Identifies an email alias supported by the HIBP domain verification API.
/// </summary>
public enum DomainVerificationEmailAlias
{
    /// <summary>
    /// The <c>admin</c> alias.
    /// </summary>
    Admin,

    /// <summary>
    /// The <c>hostmaster</c> alias.
    /// </summary>
    Hostmaster,

    /// <summary>
    /// The <c>info</c> alias.
    /// </summary>
    Info,

    /// <summary>
    /// The <c>security</c> alias.
    /// </summary>
    Security,

    /// <summary>
    /// The <c>webmaster</c> alias.
    /// </summary>
    Webmaster
}
