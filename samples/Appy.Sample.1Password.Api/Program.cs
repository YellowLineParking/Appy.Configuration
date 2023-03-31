using Appy.Configuration.OnePassword;
using Appy.Sample.OnePassword.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    // Options to load your project settings
    // 1 - Load without any extension
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

    if (builder.Environment.IsDevelopment())
    {
        // Options to load 1Password settings

        // 1.1  Load 1Password settings for the environment with the values manually set for this session:
        //builder.Configuration.Add1Password(
        //    appName: "Appy.Sample.1Password.Api",
        //    shorthand: "appyway",
        //    vault: "Development",
        //    environment: "QA",
        //    sessionToken: "1Password session token obtained with the command line");

        // 1.2  Load 1Password settings following the Appy conventions with Appy 1Password Tool
        // to initialize a session for your organization with the pre-configured environment and vault.
        builder.Configuration.Add1Password(
            appName: "Appy.Sample.1Password.Api");

        // 1.3  Load 1Password settings following the Appy conventions with the Appy 1Password Tool
        // to initialize a session for your organization with given a custom environment and vault.
        //builder.Configuration.Add1Password(
        //   appName: "Appy.Sample.1Password.Api", source =>
        //   {
        //       source.Vault = "Private";
        //       source.Environment = "Local";
        //   });
    }

    // 2 - Using a custom extension with pre-configured settings for your organization that follows the Appy conventions
    // .AddYourOrgAppConfiguration(appName: "Appy.Sample.1Password.Api")

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

await app.RunAsync();