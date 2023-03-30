using System;
using System.Net.Http;
using Appy.Configuration.Common;
using Appy.Configuration.IO;
using Appy.Configuration.Logging;
using Appy.Configuration.Serializers;
using Appy.Infrastructure.OnePassword.ApiClient;
using Appy.Infrastructure.OnePassword.Storage;
using Appy.Infrastructure.OnePassword.Tooling;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.OnePassword.Internals;

internal static class DependencyRegistration
{
    internal static IServiceCollection AddConfigurationDependencies(
        this IServiceCollection services,
        OnePasswordApiClientSettings onePasswordApiClientSettings)
    {
        var useLocalTool = string.IsNullOrWhiteSpace(onePasswordApiClientSettings.Uri);

        services
            .AddSingleton<ILogger, EmptyLogger>()
            .AddSingleton<IAppyJsonSerializer, NewtonsoftAppyJsonSerializer>()
            .AddSingleton<IProcessRunner, DefaultProcessRunner>()
            .AddSingleton<IEnvironmentAccessor, EnvironmentAccessor>()
            .AddSingleton<IPlatformInformation, PlatformInformation>()
            .AddSingleton<OnePasswordEnvironmentSessionStorage>()
            .AddSingleton<OnePasswordFileSessionStorage>()
            .AddIfElse(useLocalTool,
                s => s
                    .AddSingleton<IOnePasswordSessionStorage>(sp => new OnePasswordSessionStorageSelector(
                        sp.GetService<IPlatformInformation>(),
                        sp.GetService<OnePasswordEnvironmentSessionStorage>(),
                        sp.GetService<OnePasswordFileSessionStorage>())),
                s => s
                    .AddSingleton<IOnePasswordSessionStorage, OnePasswordSessionEmptyStorage>())
            .AddOnePasswordToolDependencies(onePasswordApiClientSettings);

        return services;
    }

    internal static IServiceCollection AddOnePasswordToolDependencies(
        this IServiceCollection services,
        OnePasswordApiClientSettings onePasswordApiClientSettings)
    {
        var useLocalTool = string.IsNullOrWhiteSpace(onePasswordApiClientSettings.Uri);

        services
            .AddHttpClient<OnePasswordApiClient>(client => client
                .WithBaseAddress(onePasswordApiClientSettings.Uri));

        services
            .AddSingleton<OnePasswordApiClientSettings>()
            .AddSingleton<IOnePasswordApiClientFactory>(sp => new OnePasswordApiClientFactory(
                sp.GetRequiredService<IHttpClientFactory>(),
                sp.GetRequiredService<IAppyJsonSerializer>()))
            .AddIfElse(useLocalTool,
                s => s.AddSingleton<IOnePasswordTool, OnePasswordLocalTool>(),
                s => s.AddSingleton<IOnePasswordTool, OnePasswordRemoteTool>());

        return services;
    }

    static HttpClient WithBaseAddress(this HttpClient client, string baseAddress)
    {
        client.BaseAddress = new Uri(baseAddress);
        return client;
    }
}