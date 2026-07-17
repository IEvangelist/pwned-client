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
    [HttpPost]
    public Task<PwnedPassword> GetPwnedPassword(
        [FromBody] PasswordLookupRequest request) =>
        request.UseNtlm
            ? pwnedPasswordsClient.GetPwnedPasswordWithNtlmAsync(
                request.Password,
                request.AddPadding)
            : pwnedPasswordsClient.GetPwnedPasswordAsync(
                request.Password,
                request.AddPadding);
}

public sealed record PasswordLookupRequest(
    string Password,
    bool AddPadding = true,
    bool UseNtlm = false);
