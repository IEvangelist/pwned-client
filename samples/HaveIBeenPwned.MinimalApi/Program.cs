// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPwnedServices(
    builder.Configuration.GetSection(nameof(HibpOptions)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

using var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

// Map "have i been pwned" breaches.
app.MapGroup("api/breaches")
   .MapPwnedBreachesApi();

// Map "have i been pwned" passwords.
app.MapPost("api/passwords",
    static (PasswordLookupRequest request, IPwnedPasswordsClient client) =>
        request.UseNtlm
            ? client.GetPwnedPasswordWithNtlmAsync(
                request.Password,
                request.AddPadding)
            : client.GetPwnedPasswordAsync(
                request.Password,
                request.AddPadding));

// Map "have i been pwned" pastes.
app.MapGet("api/pastes/{account}",
    static (string account, IPwnedPastesClient client) =>
        client.GetPastesAsync(account));

await app.RunAsync();

internal sealed record PasswordLookupRequest(
    string Password,
    bool AddPadding = true,
    bool UseNtlm = false);
