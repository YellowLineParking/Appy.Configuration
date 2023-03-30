using Appy.Configuration.Common;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Scheduling;
using Appy.Configuration.Serializers;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Appy.Tool.OnePassword.Api;
using Appy.Tool.OnePassword.Cli;
using Appy.Tool.OnePassword.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Tool.OnePassword.Composition;

public static class DependencyRegistration
{
    static IServiceCollection AddCommonDependencies(this IServiceCollection services)
    {
        services
            .AddSingleton<ILogger, ConsoleLogger>()
            .AddSingleton<IAppyJsonSerializer, NewtonsoftAppyJsonSerializer>()
            .AddSingleton<IProcessRunner, DefaultProcessRunner>()
            .AddSingleton<IEnvironmentAccessor, EnvironmentAccessor>()
            .AddSingleton<IPlatformInformation, PlatformInformation>()
            .AddSingleton<OnePasswordEnvironmentSessionStorage>()
            .AddSingleton<OnePasswordFileSessionStorage>()
            .AddSingleton<IOnePasswordSessionStorage>(sp => new OnePasswordSessionStorageSelector(
                sp.GetService<IPlatformInformation>(),
                sp.GetService<OnePasswordEnvironmentSessionStorage>(),
                sp.GetService<OnePasswordFileSessionStorage>()))
            .AddSingleton<IValidationProvider, DefaultValidationProvider>()
            .AddSingleton<IValidator, DefaultValidator>();

        services
            .AddSingleton<IValidator<GetOnePasswordNoteQuery>, GetOnePasswordNoteQueryValidator>()
            .AddSingleton<IValidator<GetOnePasswordVaultsQuery>, GetOnePasswordVaultsQueryValidator>()
            .AddSingleton<IValidator<SignInOnePasswordCommand>, SignInOnePasswordCommandValidator>();

        return services;
    }

    public static IServiceCollection AddToolDependencies(this IServiceCollection services)
    {
        services
            .AddCommonDependencies()
            .AddSingleton<ICommandLineApplicationFactory, CommandLineApplicationFactory>()
            .AddSingleton<IJobScheduler, SimpleJobScheduler>()
            .AddSingleton<IOnePasswordTool>(sp => new OnePasswordLocalTool(
                    sp.GetService<ILogger>(),
                    sp.GetService<IAppyJsonSerializer>(),
                    sp.GetService<IProcessRunner>())
                .WithValidation(sp.GetService<IValidator>()))
            .AddSingleton<IAppyOnePasswordToolCli, AppyOnePasswordToolCli>()
            .AddSingleton<IOnePasswordApiRunner, OnePasswordApiRunner>()
            .AddSingleton<IConsoleVisualzer, ConsoleVisualizer>();

        return services;
    }

    public static IServiceCollection AddApiDependencies(this IServiceCollection services)
    {
        services
            .AddCommonDependencies()
            .AddSingleton<IOnePasswordTool>(sp => new OnePasswordLocalTool(
                    sp.GetService<ILogger>(),
                    sp.GetService<IAppyJsonSerializer>(),
                    sp.GetService<IProcessRunner>())
                .WithValidation(sp.GetService<IValidator>())
                .WithConfiguration(sp.GetService<IOnePasswordSessionStorage>()));

        return services;
    }
}