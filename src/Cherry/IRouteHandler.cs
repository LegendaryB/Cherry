using System.Net;

namespace Cherry
{
    public interface IRouteHandler
    {
        Task HandleRequestAsync(
            HttpListenerContext context);
    }
}