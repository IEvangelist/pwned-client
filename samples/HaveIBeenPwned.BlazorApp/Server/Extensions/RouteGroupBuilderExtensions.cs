namespace HaveIBeenPwned.BlazorApp.Server.Extensions;

internal static class RouteGroupBuilderExtensions
{
    internal static RouteGroupBuilder MapPwnedBreachesApi(this RouteGroupBuilder group)
    {
        group.RequireAuthorization();
        group.MapGet("{breachName}",
            (string breachName, IPwnedBreachesClient client) => client.GetBreachAsync(breachName));
        group.MapGet("headers/{domain}",
            (string? domain, IPwnedBreachesClient client) => client.GetBreachAsync(domain!));
        group.MapGet("{account}/breaches",
            (string account, IPwnedBreachesClient client) => client.GetBreachesForAccountAsync(account));
        group.MapGet("{account}/headers",
            (string account, IPwnedBreachesClient client) => client.GetBreachHeadersForAccountAsync(account));
        group.MapGet("dataclasses",
            (IPwnedBreachesClient client) => client.GetDataClassesAsync());

        return group;
    }
}
