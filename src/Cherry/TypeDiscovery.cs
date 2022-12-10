using System.Reflection;

namespace Cherry
{
    public static class TypeDiscovery
    {
        private static readonly Type _controllerType = typeof(HttpController);
        private static readonly Type _middlewareType = typeof(IMiddleware);

        public static IEnumerable<Type> FindControllerTypes(Assembly assembly)
        {
            return FindTypes(assembly, _controllerType);
        }

        public static IEnumerable<Type> FindMiddlewareTypes(Assembly assembly)
        {
            return FindTypes(assembly, _middlewareType);
        }

        private static IEnumerable<Type> FindTypes(
            Assembly assembly,
            Type type)
        {
            return assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(type));
        }

        public static bool TryGetControllerRouteFromAttribute(Type type, out string path)
        {
            path = string.Empty;

            var attribute = type.GetCustomAttribute<RouteAttribute>();
            var attributePath = attribute?.Path?.ToLower();

            if (!string.IsNullOrWhiteSpace(attributePath))
                path = attributePath;

            return string.IsNullOrWhiteSpace(path);
        }
    }
}
