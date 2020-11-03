using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration
{
    public static class ServiceCollectionDescriptorExtensions
    {
        public static IServiceCollection Replace<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> factory,
            ServiceLifetime lifetime)
            where TService : class
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

            services.Remove(descriptorToRemove);

            var descriptorToAdd = new ServiceDescriptor(typeof(TService), factory, lifetime);

            services.Add(descriptorToAdd);

            return services;
        }

        public static IServiceCollection Replace<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

            services.Remove(descriptorToRemove);

            var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);

            services.Add(descriptorToAdd);

            return services;
        }

        public static IServiceCollection ReplaceSingleton<TService, TImplementation>(
            this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            return services.Replace<TService, TImplementation>(ServiceLifetime.Singleton);
        }

        public static IServiceCollection ReplaceSingleton<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> factory)
            where TService : class
        {
            return services.Replace(factory, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddIfElse(
            this IServiceCollection services,
            bool condition,
            Func<IServiceCollection, IServiceCollection> ifAction,
            Func<IServiceCollection, IServiceCollection> elseAction)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (ifAction is null)
            {
                throw new ArgumentNullException(nameof(ifAction));
            }

            if (elseAction is null)
            {
                throw new ArgumentNullException(nameof(elseAction));
            }

            return condition ? ifAction(services) : elseAction(services);
        }
    }
}