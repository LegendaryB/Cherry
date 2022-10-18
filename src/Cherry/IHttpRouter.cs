using System.Net;

namespace Cherry
{
    public interface IHttpRouter
    {
        Task RouteAsync(HttpListenerContext ctx);

        void RegisterController<TController>(
            string path,
            bool overrideExistingRoute = false)

            where TController : HttpController, new();

        void RegisterController<TController>(
            string path,
            TController controller,
            bool overrideExistingRoute = false)

            where TController : HttpController;
    }
}
