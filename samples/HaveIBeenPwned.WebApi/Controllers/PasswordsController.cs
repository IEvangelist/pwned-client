// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HaveIBeenPwned.WebApi.Controllers;

[ApiController]
[Route("api/passwords")]
public class PasswordsController(IPwnedPasswordsClient pwnedPasswordsClient) : ControllerBase
{
    [HttpGet, Route("{plainTextPassword}")]
    public Task<PwnedPassword> GetPwnedPassword([FromRoute] string plainTextPassword) => pwnedPasswordsClient.GetPwnedPasswordAsync(plainTextPassword);
}
