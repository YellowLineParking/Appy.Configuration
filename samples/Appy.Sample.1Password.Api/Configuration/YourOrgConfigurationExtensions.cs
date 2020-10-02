using Appy.Configuration.OnePassword;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Appy.Sample.OnePassword.Api.Configuration
{
    public static class YourOrgConfigurationExtensions
    {
        public static IHostBuilder AddYourOrgAppConfiguration(this IHostBuilder hostBuilder, string appName)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddYourOrgConfigurationBuilders(hostingContext.HostingEnvironment, appName);
            });

            return hostBuilder;
        }

        public static IConfigurationBuilder AddYourOrgConfigurationBuilders(this IConfigurationBuilder builder, IHostEnvironment env, string appName)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.Add1Password(appName);
            }

            return builder;
        }
    }
}
