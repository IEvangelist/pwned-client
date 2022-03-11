// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HaveIBeenPwned.WebApi.Controllers;

[ApiController]
[Route("api/breaches")]
public class BreachesController : ControllerBase
{
    private readonly IPwnedBreachesClient _pwnedBreachesClient;

    public BreachesController(IPwnedBreachesClient pwnedBreachesClient) =>
        _pwnedBreachesClient = pwnedBreachesClient;

    [HttpGet, Route("{breachName}")]
    public Task<BreachDetails?> GetBreach([FromRoute] string breachName) =>
        _pwnedBreachesClient.GetBreachAsync(breachName);

    [HttpGet, Route("headers/{domain}")]
    public Task<BreachHeader[]> GetBreaches([FromRoute] string? domain) =>
        _pwnedBreachesClient.GetBreachesAsync(domain);

    [HttpGet, Route("{account}/breaches")]
    public Task<BreachDetails[]> GetBreachesForAccount([FromRoute] string account) =>
        _pwnedBreachesClient.GetBreachesForAccountAsync(account);

    [HttpGet, Route("{account}/headers")]
    public Task<BreachHeader[]> GetBreachHeadersForAccount([FromRoute] string account) =>
        _pwnedBreachesClient.GetBreachHeadersForAccountAsync(account);

    [HttpGet, Route("dataclasses")]
    public Task<string[]> GetDataClasses() =>
        _pwnedBreachesClient.GetDataClassesAsync();
}
