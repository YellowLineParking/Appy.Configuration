using Appy.Configuration.IO;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.CLI;
using Appy.Tool.OnePassword.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword.Composition
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddToolDependencies(this IServiceCollection services)
        {
            services
                .AddSingleton<IAppyJsonSerializer, NewtonsoftAppyJsonSerializer>()
                .AddSingleton<IProcessRunner, DefaultProcessRunner>()
                .AddSingleton<IOnePasswordTool, OnePasswordTool>()
                .AddSingleton<ILogger, ConsoleLogger>()
                .AddSingleton<IOnePasswordUserEnvironmentAccessor, OnePasswordUserEnvironmentAccessor>()
                .AddSingleton<IAppyOnePasswordToolCLI, AppyOnePasswordToolCLI>();

            return services;
        }
    }
}