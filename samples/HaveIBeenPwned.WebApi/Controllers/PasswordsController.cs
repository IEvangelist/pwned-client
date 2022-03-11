// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HaveIBeenPwned.WebApi.Controllers;

[ApiController]
[Route("api/passwords")]
public class PasswordsController : ControllerBase
{
    private readonly IPwnedPasswordsClient _pwnedPasswordsClient;

    public PasswordsController(IPwnedPasswordsClient pwnedPasswordsClient) =>
        _pwnedPasswordsClient = pwnedPasswordsClient;

    [HttpGet, Route("{plainTextPassword}")]
    public Task<PwnedPassword> GetPwnedPassword([FromRoute] string plainTextPassword) =>
        _pwnedPasswordsClient.GetPwnedPasswordAsync(plainTextPassword);
}
