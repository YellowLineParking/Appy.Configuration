using Appy.Configuration.WinRegistry;
using Appy.Sample.WinRegistry.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

await app.RunAsync();