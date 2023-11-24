using Appy.Configuration.WinRegistry;
using Appy.Sample.WinRegistry.Api.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register your configuration providers

builder.Configuration.Sources.Clear();

builder.Configuration
    // Options to load your project settings
    // 1 - Load without any extension
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration
            .AddRegistrySection(() => Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\Settings");
    }

    // 2 - Using a custom extension with pre-configured settings for your organization
    // .AddYourOrgAppConfiguration()


// Load app settings

var databaseSettings = new DatabaseSettings();

builder.Configuration.GetSection("Database").Bind(databaseSettings);

// Register dependencies

builder.Services.AddControllers();

var app = builder.Build();

// Configure the app

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .UseRouting()
    .UseHttpsRedirection()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

await app.RunAsync();