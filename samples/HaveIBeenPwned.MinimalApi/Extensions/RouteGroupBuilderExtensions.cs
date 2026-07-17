// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.MinimalApi.Extensions;

internal static class RouteGroupBuilderExtensions
{
    internal static RouteGroupBuilder MapPwnedBreachesApi(this RouteGroupBuilder group)
    {
        group.MapGet("/{breachName}",
            (string breachName, IPwnedBreachesClient client) => client.GetBreachAsync(breachName));
        group.MapGet("/headers/{domain}",
            (string? domain, IPwnedBreachesClient client) => client.GetBreachesAsync(domain));
        group.MapGet("/{account}/breaches",
            (string account, IPwnedBreachesClient client) => client.GetBreachesForAccountAsync(account));
        group.MapGet("/{account}/headers",
            (string account, IPwnedBreachesClient client) => client.GetBreachHeadersForAccountAsync(account));
        group.MapGet("/{account}/headers/k-anonymity",
            (string account, IPwnedBreachesClient client) =>
                client.GetBreachHeadersForAccountUsingKAnonymityAsync(account));
        group.MapGet("/dataclasses",
            (IPwnedBreachesClient client) => client.GetDataClassesAsync());

        return group;
    }
}
