using HaveIBeenPwned.Client;
using HaveIBeenPwned.Client.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPwnedServices(
    builder.Configuration.GetSection(nameof(HibpOptions)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new() { Title = "HaveIBeenPwned.MinimalApi", Version = "v1" }));

using var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HaveIBeenPwned.MinimalApi v1"));
}

app.UseHttpsRedirection();

// "Have I Been Pwned" Breaches API
app.MapGet("api/breaches/{breachName}",
    async (string breachName, IPwnedBreachesClient client) =>
    await client.GetBreachAsync(breachName));
app.MapGet("api/breaches/headers/{domain}",
    async (string? domain, IPwnedBreachesClient client) =>
    await client.GetBreachAsync(domain!));
app.MapGet("api/breaches/{account}/breaches",
    async (string account, IPwnedBreachesClient client) =>
    await client.GetBreachesForAccountAsync(account));
app.MapGet("api/breaches/{account}/headers",
    async (string account, IPwnedBreachesClient client) =>
    await client.GetBreachHeadersForAccountAsync(account));
app.MapGet("api/breaches/dataclasses",
    async (IPwnedBreachesClient client) =>
    await client.GetDataClassesAsync());

// "Have I Been Pwned" Pwned Passwords API
app.MapGet("api/passwords/{plainTextPassword}",
    async (string plainTextPassword, IPwnedPasswordsClient client) =>
        await client.GetPwnedPasswordAsync(plainTextPassword));

// "Have I Been Pwned" Pwned Pastes API
app.MapGet("api/pastes/{account}",
    async (string account, IPwnedPastesClient client) =>
    await client.GetPastesAsync(account));

await app.RunAsync();
