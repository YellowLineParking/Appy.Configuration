using Appy.Configuration.OnePassword;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Appy.Sample.OnePassword.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                // Options to load your settings

                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    // 1 - Load without any extension

                    builder
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        // Options to load 1Password settings

                        // 1.1  Load 1Password settings for the environment with the values manually setted for this session:
                        builder.Add1Password(
                            appName: "Appy.Sample.1Password.Api",
                            organization: "appyway",
                            vault: "Development",
                            environment: "QA",
                            sessionToken: "1Password session token obtained with the command line");

                        // 1.2  Load 1Password settings following the Appy conventions and the Appy external Tool
                        // to initialize a session for your organization and environment:
                        // builder.Add1Password(
                        //    appName: "Appy.Sample.1Password.Api");
                    }
                })

                // 2 - Using a custom extension with pre-configured settings for your organization that follows the Appy conventions
                // .AddYourOrgAppConfiguration(appName: "Appy.Sample.1Password.Api")

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
