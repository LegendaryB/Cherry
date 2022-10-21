using Cherry.Extensions;
using Cherry.Middleware;
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

        /// <summary>
        /// Allows to use a custom router implementation.
        /// </summary>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer UseRouter<TRouter>()
            where TRouter : class, IHttpRouter, new()
        {
            return UseRouter(new TRouter());
        }

        /// <summary>
        /// Allows to use a custom router implementation.
        /// </summary>
        /// <param name="router">The <see cref="IHttpRouter"/> instance to use</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer UseRouter<TRouter>(TRouter router)
            where TRouter : class, IHttpRouter
        {
            _router = router ?? throw new ArgumentNullException(nameof(router));
            return this;
        }

        /// <summary>
        /// Wraps a <see cref="Func{T1, T2, TResult}"/> into a (internal) middleware class and registers it for the given route.
        /// </summary>
        /// <param name="handler">The <see cref="Func{T1, T2, TResult}"/> which should be wrapped into an (internal) middleware class.</param>
        /// <param name="route">The route on which the middleware should be registered.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterMiddleware(
            Func<HttpRequest, HttpResponse, Task> handler,
            string route = "*")
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            var middleware = new FuncAsMiddleware(handler);

            return RegisterMiddleware(
                middleware,
                route);
        }

        /// <summary>
        /// Allows to register a middleware used globally or bound to a specific route.
        /// </summary>
        /// <param name="route">The route on which the middleware should be registered.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterMiddleware<TMiddleware>(string route = "*")
            where TMiddleware : class, IMiddleware, new()
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            return RegisterMiddleware(
                new TMiddleware(),
                route);
        }

        /// <summary>
        /// Allows to register a middleware used globally or bound to a specific route.
        /// </summary>
        /// <param name="middleware"></param>
        /// <param name="route">The route on which the middleware should be registered.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterMiddleware<TMiddleware>(
            TMiddleware middleware,
            string route = "*")

            where TMiddleware : class, IMiddleware
        {
            if (middleware is null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            _router.RegisterMiddleware(
                route, 
                middleware);

            return this;
        }

        /// <summary>
        /// Constructs and registers a controller for the route declared with the <see cref="RouteAttribute"/>.
        /// </summary>
        /// <param name="overrideExistingRoute">Flag to indicate that a route (if already registered) should be overriden.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterController<TController>(bool overrideExistingRoute = false)
            where TController : HttpController, new()
        {
            return RegisterController(
                new TController(), 
                overrideExistingRoute);
        }

        /// <summary>
        /// Registers a controller and extracts the route from the declared <see cref="RouteAttribute"/>.
        /// </summary>
        /// <param name="controller">A instance which must be assignable to the type <see cref="HttpController"/>.</param>
        /// <param name="overrideExistingRoute">Flag to indicate that a route (if already registered) should be overriden.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterController<TController>(
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController
        {
            var route = ControllerDiscovery.GetPathFromRouteAttribute(
                typeof(TController));

            if (string.IsNullOrWhiteSpace(route))
                throw new InvalidOperationException($"Failed to resolve controller route!");

            return RegisterController(
                route,
                controller,
                overrideExistingRoute);
        }

        /// <summary>
        /// Wraps a <see cref="Func{T, TResult}"/> into a (internal) controller class and registers it for the given route.
        /// </summary>
        /// <param name="route">The route which should be handled by the controller.</param>
        /// <param name="handler">The <see cref="Func{T, TResult}"/> which should be wrapped into an (internal) controller class.</param>
        /// <param name="overrideExistingRoute">Flag to indicate that a route (if already registered) should be overriden.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterController(
            string route,
            Func<HttpRequest, HttpResponse, Task> handler,
            bool overrideExistingRoute = false)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return RegisterController(
                route,
                new FuncAsHttpController(handler),
                overrideExistingRoute);
        }

        /// <summary>
        /// Constructs and registers a controller for the given route.
        /// </summary>
        /// <param name="route">The route which should be handled by the controller.</param>
        /// <param name="overrideExistingRoute">Flag to indicate that a route (if already registered) should be overriden.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterController<TController>(
            string route,
            bool overrideExistingRoute = false)

            where TController : HttpController, new()
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            return RegisterController(
                route, 
                new TController(), 
                overrideExistingRoute);
        }

        /// <summary>
        /// Registers a controller for the given route.
        /// </summary>
        /// <param name="route">The route which should be handled by the controller.</param>
        /// <param name="controller">A instance which must be assignable to the type parameter <typeparamref name="TController"/>.</param>
        /// <param name="overrideExistingRoute">Flag to indicate that a route (if already registered) should be overriden.</param>
        /// <returns>The current <see cref="HttpServer"/> instance.</returns>
        public HttpServer RegisterController<TController>(
            string route, 
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException($"'{nameof(route)}' cannot be null or whitespace.", nameof(route));
            }

            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            _router.RegisterController(
                route,
                controller,
                overrideExistingRoute);

            return this;
        }

        /// <summary>
        /// Auto registers all types inheriting from <see cref="HttpController"/> and using a <see cref="RouteAttribute"/> to declare a path.
        /// </summary>
        public void AutoRegisterControllers()
        {
            AutoRegisterControllers(
                Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Auto registers all types inheriting from <see cref="HttpController"/> and using a <see cref="RouteAttribute"/> to declare a path.
        /// </summary>
        public void AutoRegisterControllers(Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            ControllerDiscovery.FindAndRegisterControllers(
                assembly,
                _router);
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