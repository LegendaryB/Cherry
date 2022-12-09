﻿using System.Reflection;

namespace Cherry.Controller
{
    internal static class HttpControllerDiscovery
    {
        private static readonly Type _controllerType = typeof(IHttpController);

        internal static IEnumerable<Type> FindControllerTypes(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(_controllerType));
        }

        internal static bool TryGetControllerRouteFromAttribute(Type type, out string path)
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
