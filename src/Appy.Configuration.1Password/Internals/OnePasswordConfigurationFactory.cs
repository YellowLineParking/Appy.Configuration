using System;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.OnePassword.Internals
{
    internal static class OnePasswordConfigurationFactory
    {
        internal static Action<IServiceCollection> ServiceInterceptor;

        internal static IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddConfigurationDependencies();
            
            ServiceInterceptor?.Invoke(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }
    }
}