using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Cherry.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddControllers(
            this IServiceCollection services,
            ServiceLifetime lifetime)
        {
            services.AddControllers(
                Assembly.GetCallingAssembly(),
                lifetime);
        }

        public static void AddControllers(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var controllerTypes = HttpControllerDiscovery.FindControllerTypes(assembly);

            foreach (var controllerType in controllerTypes)
            {
                services.AddController(
                    controllerType,
                    lifetime);
            }
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

        private static IServiceCollection AddController(
            this IServiceCollection services, 
            Type controllerType, 
            ServiceLifetime lifetime)
        {
            services.Add(
                new ServiceDescriptor(
                    serviceType: controllerType,
                    implementationType: controllerType,
                    lifetime: lifetime));

            return services;
        }
    }
}
