// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

var group = app.MapGroup("api/breaches")
    .RequireAuthorization();

// Map "have i been pwned" breaches.
app.MapGet("{breachName}",
    (string breachName, IPwnedBreachesClient client) => client.GetBreachAsync(breachName));
app.MapGet("headers/{domain}",
    (string? domain, IPwnedBreachesClient client) => client.GetBreachAsync(domain!));
app.MapGet("{account}/breaches",
    (string account, IPwnedBreachesClient client) => client.GetBreachesForAccountAsync(account));
app.MapGet("{account}/headers",
    (string account, IPwnedBreachesClient client) => client.GetBreachHeadersForAccountAsync(account));
app.MapGet("dataclasses",
    (IPwnedBreachesClient client) => client.GetDataClassesAsync());

// Map "have i been pwned" passwords.
app.MapGet("api/passwords/{plainTextPassword}",
    [Authorize] (string plainTextPassword, IPwnedPasswordsClient client) => client.GetPwnedPasswordAsync(plainTextPassword));

// Map "have i been pwned" pastes.
app.MapGet("api/pastes/{account}",
    [Authorize] (string account, IPwnedPastesClient client) => client.GetPastesAsync(account));

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
