using System.Net;

namespace Cherry
{
    internal class FuncRouteHandler : IRouteHandler
    {
        private readonly Func<HttpListenerContext, Task> _fn;

        public FuncRouteHandler(Func<HttpListenerContext, Task> fn)
        {
            _fn = fn;
        }

        public Task HandleRequestAsync(HttpListenerContext context)
        {
            return _fn.Invoke(context);
        }
    }
}