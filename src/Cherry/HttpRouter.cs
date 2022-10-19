using Cherry.Extensions;
using Cherry.Middleware;

using Microsoft.Extensions.Logging;

using System.Net;

namespace Cherry
{
    internal class HttpRouter : IHttpRouter
    {
        private const string GLOBAL_MIDDLEWARE_KEY = "*";

        private readonly ILogger _logger;

        private readonly Dictionary<string, HttpController> _routingTable;
        private readonly List<KeyValuePair<string, List<IMiddleware>>> _middlewareTable;

        public HttpRouter(
            ILogger logger)
        {
            _logger = logger;
            _routingTable = new();
            _middlewareTable = new();
        }

        private List<IMiddleware> GetMiddlewaresForRoute(string route)
        {
            var middlewareEntry = _middlewareTable
                .FirstOrDefault(entry => entry.Key.Equals(route));

            if (middlewareEntry.Equals(default(KeyValuePair<string, List<IMiddleware>>)))
                return Enumerable.Empty<IMiddleware>().ToList();

            return middlewareEntry.Value;
        }

        private async Task InvokeMiddlewaresAsync(
            HttpListenerContext ctx,
            string route)
        {
            var middlewares = GetMiddlewaresForRoute(GLOBAL_MIDDLEWARE_KEY);

            middlewares.AddRange(
                GetMiddlewaresForRoute(route));

            foreach (var middleware in middlewares)
            {
                await middleware.HandleRequestAsync(
                    ctx.Request, 
                    ctx.Response);
            }
        }

        public async Task RouteAsync(HttpListenerContext ctx)
        {
            try
            {
                _logger.LogInformation($"{nameof(RouteAsync)} ENTER");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var route = ctx.Request.Url.LocalPath;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                if (string.IsNullOrWhiteSpace(route))
                {
                    _logger.LogWarning($"'{nameof(route)}' cannot be null or whitespace.");

                    await ctx.Response.AnswerWithStatusCodeAsync(HttpStatusCode.InternalServerError);
                    return;
                }

                route = route.ToLower();

                if (!_routingTable.TryGetValue(route, out var controller))
                {
                    _logger.LogWarning($"Failed to resolve controller for route '{route}'.");

                    await ctx.Response.AnswerWithStatusCodeAsync(HttpStatusCode.InternalServerError);
                    return;
                }

                await InvokeMiddlewaresAsync(
                    ctx,
                    route);

                await controller.HandleAnyAsync(ctx);
            }
            finally
            {
                _logger.LogInformation($"{nameof(RouteAsync)} LEAVE");
            }
        }

        public void RegisterMiddleware<TMiddleware>(
            string route,
            TMiddleware middleware)

            where TMiddleware : class, IMiddleware
        {
            var entry = _middlewareTable.FirstOrDefault(entry => entry.Key.Equals(route));

            if (!entry.Equals(default(KeyValuePair<string, List<IMiddleware>>)))
            {
                entry.Value.Add(middleware);
                return;
            }

            entry = new KeyValuePair<string, List<IMiddleware>>(
                route,
                new List<IMiddleware> { middleware });

            _middlewareTable.Add(entry);
        }

        public void RegisterController<TController>(
            string route,
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController
        {
            try
            {
                _logger.LogInformation($"{nameof(RegisterController)} ENTER");

                if (_routingTable.ContainsKey(route) && !overrideExistingRoute)
                    throw new ArgumentException($"There is already a controller registered for the route '{route}'!");

                _routingTable.Add(
                    route,
                    controller);
            }
            finally
            {
                _logger.LogInformation($"{nameof(RegisterController)} LEAVE");
            }
        }
    }
}
