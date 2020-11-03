using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.OnePassword.Internals
{
    internal static class DependencyRegistration
    {
        internal static IServiceCollection AddConfigurationDependencies(this IServiceCollection services)
        {
            services
                .AddSingleton<ILogger, EmptyLogger>()
                .AddSingleton<IAppyJsonSerializer, NewtonsoftAppyJsonSerializer>()
                .AddSingleton<IProcessRunner, DefaultProcessRunner>()
                .AddSingleton<IOnePasswordTool, OnePasswordTool>()
                .AddSingleton<OnePasswordEnvironmentSessionStorage>()
                .AddSingleton<OnePasswordFileSessionStorage>()
                .AddSingleton<IOnePasswordSessionStorage>(sp => new OnePasswordSessionStorageSelector(
                    sp.GetService<OnePasswordEnvironmentSessionStorage>(),
                    sp.GetService<OnePasswordFileSessionStorage>()));

            return services;
        }
    }
}