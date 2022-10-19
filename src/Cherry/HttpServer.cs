using Cherry.Extensions;
using Cherry.Routing;

using Microsoft.Extensions.Logging;

using System.Net;
using System.Reflection;

namespace Cherry
{
    public class HttpServer
    {
        private readonly HttpListener _httpListener;
        private readonly ILogger _logger;

        private IHttpRouter _router;

        public HttpServer(
            ILogger logger,
            HttpListener? httpListener)
        {
            _logger = logger;
            _httpListener = httpListener ?? (new());
            _router = new HttpRouter(logger);
        }

        public HttpServer(
            ILogger logger,
            params string[] prefixes)

            : this(logger, httpListener: null)
        {
            _httpListener.Prefixes.AddRange(prefixes.ToArray());
        }

        public HttpServer RegisterController(
            string path,
            Func<HttpListenerContext, Task> handler,
            bool overrideExistingRoute = false)
        {
            var controller = new FuncAsHttpController(handler);

            return RegisterController(
                path,
                controller,
                overrideExistingRoute);
        }

        /// <summary>
        /// Allows to use a custom router implementation.
        /// </summary>
        public HttpServer UseRouter<TRouter>()
            where TRouter : class, IHttpRouter, new()
        {
            return UseRouter(new TRouter());
        }

        /// <summary>
        /// Allows to use a custom router implementation.
        /// </summary>
        /// <param name="router">The <see cref="IHttpRouter"/> instance to use</param>
        /// <exception cref="ArgumentNullException">When the <paramref name="router"/> is null.</exception>
        public HttpServer UseRouter<TRouter>(TRouter router)
            where TRouter : class, IHttpRouter
        {
            _router = router ?? throw new ArgumentNullException(nameof(router));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="route"></param>
        /// <param name="overrideExistingRoute"></param>
        /// <returns></returns>
        public HttpServer RegisterController<TController>(
            string route,
            bool overrideExistingRoute = false)

            where TController : HttpController, new()
        {
            return RegisterController(
                route, 
                new TController(), 
                overrideExistingRoute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="path"></param>
        /// <param name="controller"></param>
        /// <param name="overrideExistingRoute"></param>
        /// <returns></returns>
        public HttpServer RegisterController<TController>(
            string path, 
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController
        {
            _router.RegisterController(
                path,
                controller,
                overrideExistingRoute);

            return this;
        }

        /// <summary>
        /// Auto registers all types inheriting from <see cref="HttpController"/> and using a <see cref="RouteAttribute"/> to declare a path.
        /// </summary>
        public void AutoRegisterControllers()
        {
            AutoRegisterControllers(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Auto registers all types inheriting from <see cref="HttpController"/> and using a <see cref="RouteAttribute"/> to declare a path.
        /// </summary>
        public void AutoRegisterControllers(Assembly assembly)
        {
            ControllerDiscovery.FindAndRegisterControllers(assembly, _router);
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
                        await _router.RouteAsync(ctx);
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