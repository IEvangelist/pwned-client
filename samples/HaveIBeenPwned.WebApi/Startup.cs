// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using Scalar.AspNetCore;

namespace HaveIBeenPwned.WebApi;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPwnedClient>(
            services => new PwnedClient(configuration["HibpOptions:ApiKey"]!));

        services.AddControllers();
        services.AddOpenApi();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference();
        });
    }
}
