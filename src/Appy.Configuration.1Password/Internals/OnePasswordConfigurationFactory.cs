using System;
using Appy.Configuration.Common;
using Appy.Infrastructure.OnePassword.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.OnePassword.Internals;

internal static class OnePasswordConfigurationFactory
{
    internal static IServiceProvider BuildServiceProvider()
    {
        var onePasswordApiClientSettings = new OnePasswordApiClientSettings
        {
            Uri = EnvironmentUtils.GetProcessEnvironmentVariable(KnownSessionVars.OnePasswordApiUri)
        };

        var serviceCollection = new ServiceCollection()
            .AddConfigurationDependencies(onePasswordApiClientSettings);

        return serviceCollection.BuildServiceProvider();
    }
}