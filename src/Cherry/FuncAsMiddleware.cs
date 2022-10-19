using Cherry.Middleware;

using System.Net;

namespace Cherry
{
    internal class FuncAsMiddleware : IMiddleware
    {
        Func<HttpListenerRequest, HttpListenerResponse, Task> _handler;

        public FuncAsMiddleware(Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
        {
            _handler = handler;
        }

        public Task HandleRequestAsync(HttpListenerRequest req, HttpListenerResponse res)
        {
            return _handler.Invoke(req, res);
        }
    }
}
