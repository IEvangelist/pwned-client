// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// An aggregate interface for convenience, exposing all
/// "pwned" functionality including:
/// <list type="bullet">
/// <item><see cref="IPwnedBreachesClient"/></item>
/// <item><see cref="IPwnedPastesClient"/></item>
/// <item><see cref="IPwnedPasswordsClient"/></item>
/// </list>
/// </summary>
public interface IPwnedClient :
    IPwnedBreachesClient,
    IPwnedPastesClient,
    IPwnedPasswordsClient
{
}
