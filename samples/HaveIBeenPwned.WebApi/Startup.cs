// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HaveIBeenPwned.Client;
using Microsoft.OpenApi.Models;

namespace HaveIBeenPwned.WebApi;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPwnedClient>(
            services => new PwnedClient(configuration["HibpOptions:ApiKey"]!));

        services.AddControllers();
        services.AddSwaggerGen(
            options =>
            options.SwaggerDoc(
                "v1", new OpenApiInfo { Title = "HaveIBeenPwned.WebApi", Version = "v1" }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HaveIBeenPwned.WebApi v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
