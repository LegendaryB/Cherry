using System.Net;

namespace Cherry.Middleware
{
    public interface IMiddleware
    {
        Task HandleRequestAsync(
            HttpListenerRequest req,
            HttpListenerResponse res);
    }
}
