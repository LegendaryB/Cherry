using Microsoft.Extensions.Logging;

using System.Net;

using Cherry.Extensions;

namespace Cherry
{
    public class HttpServer
    {
        private readonly HttpListener _httpListener;
        private readonly ILogger<HttpServer> _logger;
        private readonly Dictionary<string, IRouteHandler> _routeToHandlerMap = new();

        public HttpServer(
            ILogger<HttpServer> logger,
            HttpListener? httpListener)
        {
            _httpListener = httpListener ?? (new());
            _logger = logger;
        }

        public HttpServer(
            ILogger<HttpServer> logger,
            params string[] prefixes)

            : this(logger, httpListener: null)
        {
            _httpListener.Prefixes.AddRange(prefixes.ToArray());
        }

        public void RegisterHandlerForRoute(string route, IRouteHandler handler)
        {
            _routeToHandlerMap.Add(route, handler);
        }

        public void RegisterHandlerForRoute(string route, Func<HttpListenerContext, Task> handler)
        {
            RegisterHandlerForRoute(
                route,
                new FuncRouteHandler(handler));
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"{nameof(RunAsync)} ENTER");

                _httpListener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var ctx = await _httpListener.GetContextAsync();
                        var route = ctx.Request.RawUrl;

                        if (string.IsNullOrWhiteSpace(route))
                        {
                            ctx.Response.AnswerWithStatusCode(HttpStatusCode.NotFound);
                            continue;
                        }

                        if (!_routeToHandlerMap.TryGetValue(route, out var handler))
                        {
                            ctx.Response.AnswerWithStatusCode(HttpStatusCode.NotImplemented);
                            continue;
                        }

                        await handler.HandleRequestAsync(ctx);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Empty);
                    }
                }

                _httpListener.Stop();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw;
            }
            finally
            {
                _logger.LogInformation($"{nameof(RunAsync)} LEAVE");
            }
        }
    }
}