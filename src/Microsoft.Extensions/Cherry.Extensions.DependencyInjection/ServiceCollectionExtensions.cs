using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Cherry
{
    public static class ServiceCollectionExtensions
    {
        public static void AddControllers(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddControllers(
                Assembly.GetCallingAssembly(),
                lifetime);
        }

        public static void AddControllers(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            foreach (var assembly in assemblies)
            {
                services.AddControllers(
                    assembly,
                    lifetime);
            }
        }

        public static void AddControllers(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddServices(
                assembly,
                TypeDiscovery.FindControllerTypes,
                lifetime);
        }

        public static void AddMiddlewares(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddMiddlewares(
                Assembly.GetCallingAssembly(),
                lifetime);
        }

        public static void AddMiddlewares(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            foreach (var assembly in assemblies)
            {
                services.AddMiddlewares(
                    assembly,
                    lifetime);
            }
        }

        public static void AddMiddlewares(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddServices(
                assembly,
                TypeDiscovery.FindMiddlewareTypes,
                lifetime);
        }

        private static void AddServices(
            this IServiceCollection services,
            Assembly assembly,
            Func<Assembly, IEnumerable<Type>> typeDiscovery,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (typeDiscovery is null)
            {
                throw new ArgumentNullException(nameof(typeDiscovery));
            }

            var types = typeDiscovery.Invoke(assembly);

            foreach (var type in types)
            {
                services.AddService(
                    type,
                    lifetime);
            }
        }

        private static IServiceCollection AddService(
            this IServiceCollection services, 
            Type implementationType, 
            ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(
                    serviceType: implementationType,
                    implementationType: implementationType,
                    lifetime: lifetime));

            return services;
        }
    }
}
