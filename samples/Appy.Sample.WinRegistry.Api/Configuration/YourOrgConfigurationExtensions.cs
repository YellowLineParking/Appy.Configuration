using System;
using Appy.Configuration.WinRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Appy.Sample.WinRegistry.Api.Configuration
{
    public static class YourOrgConfigurationExtensions
    {
        public static IConfigurationBuilder AddYourOrgRegistrySection(
            this IConfigurationBuilder builder,
            Action<WinRegistryConfigurationSource> configureSource = null)
        {
            return builder.AddRegistrySection(() =>
                Microsoft.Win32.Registry.CurrentUser, "Software\\YOUR_ORG\\Settings");
        }

        public static IHostBuilder AddYourOrgAppConfiguration(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddYourOrgConfigurationBuilders(hostingContext.HostingEnvironment);
            });

            return hostBuilder;
        }

        public static IConfigurationBuilder AddYourOrgConfigurationBuilders(this IConfigurationBuilder builder, IHostEnvironment env)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder
                    .AddYourOrgRegistrySection();
            }

            return builder;
        }
    }
}
