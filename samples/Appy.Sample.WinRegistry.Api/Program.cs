using Appy.Configuration.WinRegistry;
using Appy.Sample.WinRegistry.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Appy.Sample.WinRegistry.Api
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
                        builder
                            .AddRegistrySection(() => Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\Settings");
                    }
                })

                // 2 - Using a custom extension with pre-configured settings for your organization
                // .AddYourOrgAppConfiguration()

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
