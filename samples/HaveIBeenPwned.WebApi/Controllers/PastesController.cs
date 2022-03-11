// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HaveIBeenPwned.WebApi.Controllers;

[ApiController]
[Route("api/pastes")]
public class PastesController : ControllerBase
{
    private readonly IPwnedPastesClient _pwnedPastesClient;

    public PastesController(IPwnedPastesClient pwnedPastesClient) =>
        _pwnedPastesClient = pwnedPastesClient;

    [HttpGet, Route("{account}")]
    public Task<Pastes[]> GetPaste([FromRoute] string account) =>
        _pwnedPastesClient.GetPastesAsync(account);
}
