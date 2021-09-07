﻿var builder = WebApplication.CreateBuilder(args);

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

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
