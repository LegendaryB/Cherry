using System.Net;

namespace Cherry
{
    internal class FuncAsHttpController : HttpController
    {
        private readonly Func<HttpListenerContext, Task> _handler;

        internal FuncAsHttpController(Func<HttpListenerContext, Task> handler)
        {
            _handler = handler;
        }

        internal override Task HandleAnyAsync(HttpListenerContext ctx)
        {
            return _handler.Invoke(ctx);
        }
    }
}
