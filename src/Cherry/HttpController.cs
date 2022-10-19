using Cherry.Extensions;

using System.Net;

namespace Cherry
{
    public class HttpController
    {
        protected HttpController() { }

        public virtual Task HandlePostAsync(HttpListenerContext ctx) =>
            SendDefaultResponse(ctx);

        public virtual Task HandlePutAsync(HttpListenerContext ctx) =>
            SendDefaultResponse(ctx);

        public virtual Task HandleGetAsync(HttpListenerContext ctx) =>
            SendDefaultResponse(ctx);

        public virtual Task HandleDeleteAsync(HttpListenerContext ctx) =>
            SendDefaultResponse(ctx);

        internal virtual Task HandleAnyAsync(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod == HttpMethod.Post.ToString())
                return HandlePostAsync(ctx);

            if (ctx.Request.HttpMethod == HttpMethod.Put.ToString())
                return HandlePutAsync(ctx);

            if (ctx.Request.HttpMethod == HttpMethod.Get.ToString())
                return HandleGetAsync(ctx);

            if (ctx.Request.HttpMethod == HttpMethod.Delete.ToString())
                return HandleDeleteAsync(ctx);


            return SendDefaultResponse(ctx);
        }

        protected static Task SendDefaultResponse(HttpListenerContext ctx)
        {
            ctx.Response.AnswerWithStatusCode(HttpStatusCode.NotImplemented);
            return Task.CompletedTask;
        }
    }
}
