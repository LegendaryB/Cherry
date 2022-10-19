﻿using System.Reflection;

namespace Cherry.Routing
{
    internal static class ControllerDiscovery
    {
        internal static void FindAndRegisterControllers(
            Assembly assembly,
            IHttpRouter router)
        {
            var controllerType = typeof(HttpController);

            var types = assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(controllerType));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RouteAttribute>();

                if (attribute == null)
                    continue;

                var ctor = type.GetConstructor(Type.EmptyTypes);

                if (ctor == null)
                    continue;

                if (Activator.CreateInstance(type) is not HttpController instance)
                    continue;

                router.RegisterController(
                    attribute.Path.ToLower(),
                    instance);
            }
        }
    }
}
