// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPwnedServices(
    builder.Configuration.GetSection(nameof(HibpOptions)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new() { Title = "HaveIBeenPwned.MinimalApi", Version = "v1" }));

using var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HaveIBeenPwned.MinimalApi v1"));
}

app.UseHttpsRedirection();

// Map "have i been pwned" breaches.
app.MapGet("api/breaches/{breachName}",
    (string breachName, IPwnedBreachesClient client) => client.GetBreachAsync(breachName));
app.MapGet("api/breaches/headers/{domain}",
    (string? domain, IPwnedBreachesClient client) => client.GetBreachAsync(domain!));
app.MapGet("api/breaches/{account}/breaches",
    (string account, IPwnedBreachesClient client) => client.GetBreachesForAccountAsync(account));
app.MapGet("api/breaches/{account}/headers",
    (string account, IPwnedBreachesClient client) => client.GetBreachHeadersForAccountAsync(account));
app.MapGet("api/breaches/dataclasses",
    (IPwnedBreachesClient client) => client.GetDataClassesAsync());

// Map "have i been pwned" passwords.
app.MapGet("api/passwords/{plainTextPassword}",
    (string plainTextPassword, IPwnedPasswordsClient client) => client.GetPwnedPasswordAsync(plainTextPassword));

// Map "have i been pwned" pastes.
app.MapGet("api/pastes/{account}",
    (string account, IPwnedPastesClient client) => client.GetPastesAsync(account));

await app.RunAsync();
