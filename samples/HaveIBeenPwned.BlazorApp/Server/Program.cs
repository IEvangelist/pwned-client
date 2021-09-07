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

// Map "have i been pwned" breaches.
app.MapGet("api/breaches/{breachName}",
    [Authorize] (string breachName, IPwnedBreachesClient client) => client.GetBreachAsync(breachName));
app.MapGet("api/breaches/headers/{domain}",
    [Authorize] (string? domain, IPwnedBreachesClient client) => client.GetBreachAsync(domain!));
app.MapGet("api/breaches/{account}/breaches",
    [Authorize] (string account, IPwnedBreachesClient client) => client.GetBreachesForAccountAsync(account));
app.MapGet("api/breaches/{account}/headers",
    [Authorize] (string account, IPwnedBreachesClient client) => client.GetBreachHeadersForAccountAsync(account));
app.MapGet("api/breaches/dataclasses",
    [Authorize] (IPwnedBreachesClient client) => client.GetDataClassesAsync());

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
