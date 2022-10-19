using Cherry.Extensions;

using Microsoft.Extensions.Logging;

using System.Net;

namespace Cherry
{
    internal class HttpRouter : IHttpRouter
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, HttpController> _routingTable = new();

        public HttpRouter(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task RouteAsync(HttpListenerContext ctx)
        {
            var route = ctx.Request?.Url?.LocalPath;

            if (string.IsNullOrWhiteSpace(route))
            {
                ctx.Response.AnswerWithStatusCode(HttpStatusCode.InternalServerError);
                return Task.CompletedTask;
            }

            route = route.ToLower();

            if (!_routingTable.TryGetValue(route, out var controller))
            {
                ctx.Response.AnswerWithStatusCode(HttpStatusCode.NotImplemented);
                return Task.CompletedTask;
            }

            return controller.HandleAnyAsync(ctx);
        }

        public void RegisterController<TController>(
            string route,
            bool overrideExistingRoute = false)

            where TController : HttpController, new()
        {
            RegisterController(
                route,
                new TController(),
                overrideExistingRoute);
        }

        public void RegisterController<TController>(
            string route,
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController
        {
            if (_routingTable.ContainsKey(route) && !overrideExistingRoute)
                throw new ArgumentException($"There is already a controller registered for the route '{route}'!");

            _routingTable.Add(
                route,
                controller);
        }
    }
}
