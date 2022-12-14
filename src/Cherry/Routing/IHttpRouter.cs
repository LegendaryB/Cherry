using System.Net;

namespace Cherry
{
    public interface IHttpRouter
    {
        Task RouteAsync(HttpListenerContext ctx);

        void RegisterMiddleware<TMiddleware>(
            string route,
            TMiddleware middleware)

            where TMiddleware : class, IMiddleware;

        void RegisterController<TController>(
            string path,
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController;
    }
}
