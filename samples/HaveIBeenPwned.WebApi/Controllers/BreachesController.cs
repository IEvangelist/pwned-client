// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HaveIBeenPwned.WebApi.Controllers;

[ApiController]
[Route("api/breaches")]
public class BreachesController(IPwnedBreachesClient pwnedBreachesClient) : ControllerBase
{
    [HttpGet, Route("{breachName}")]
    public Task<BreachDetails?> GetBreach([FromRoute] string breachName) =>
        pwnedBreachesClient.GetBreachAsync(breachName);

    [HttpGet, Route("headers/{domain}")]
    public Task<BreachHeader[]> GetBreaches([FromRoute] string? domain) =>
        pwnedBreachesClient.GetBreachesAsync(domain);

    [HttpGet, Route("{account}/breaches")]
    public Task<BreachDetails[]> GetBreachesForAccount([FromRoute] string account) =>
        pwnedBreachesClient.GetBreachesForAccountAsync(account);

    [HttpGet, Route("{account}/headers")]
    public Task<BreachHeader[]> GetBreachHeadersForAccount([FromRoute] string account) =>
        pwnedBreachesClient.GetBreachHeadersForAccountAsync(account);

    [HttpGet, Route("dataclasses")]
    public Task<string[]> GetDataClasses() =>
        pwnedBreachesClient.GetDataClassesAsync();
}
