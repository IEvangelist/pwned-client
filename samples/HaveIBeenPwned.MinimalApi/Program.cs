using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPwnedServices(options =>
{
    options.ApiKey = builder.Configuration["HibpOptions:ApiKey"];
    options.UserAgent = builder.Configuration["HibpOptions:UserAgent"];
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HaveIBeenPwned.MinimalApi", Version = "v1" });
});

using var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HaveIBeenPwned.MinimalApi v1"));
}

// "Have I Been Pwned" Breaches API
app.MapGet("api/breaches/{breachName}",
    async (string breachName, IPwnedBreachesClient client) =>
    Results.Ok(await client.GetBreachAsync(breachName)));
app.MapGet("api/breaches/headers/{domain}",
    async (string? domain, IPwnedBreachesClient client) =>
    Results.Ok(await client.GetBreachAsync(domain!)));
app.MapGet("api/breaches/{account}/breaches",
    async (string account, IPwnedBreachesClient client) =>
    Results.Ok(await client.GetBreachesForAccountAsync(account)));
app.MapGet("api/breaches/{account}/headers",
    async (string account, IPwnedBreachesClient client) =>
    Results.Ok(await client.GetBreachHeadersForAccountAsync(account)));
app.MapGet("api/breaches/dataclasses",
    async (IPwnedBreachesClient client) =>
    Results.Ok(await client.GetDataClassesAsync()));

// "Have I Been Pwned" Pwned Passwords API
app.MapGet("api/passwords/{plainTextPassword}",
    async (string plainTextPassword, IPwnedPasswordsClient client) =>
        Results.Ok(await client.GetPwnedPasswordAsync(plainTextPassword)));

// "Have I Been Pwned" Pwned Pastes API
app.MapGet("api/pastes/{account}",
    async (string account, IPwnedPastesClient client) =>
    Results.Ok(await client.GetPastesAsync(account)));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSwaggerUI();

await app.RunAsync();
