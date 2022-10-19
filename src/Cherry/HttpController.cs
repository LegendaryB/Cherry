using Cherry.Extensions;

using System.Net;

namespace Cherry
{
    public class HttpController
    {
        protected HttpController() { }

        public virtual Task HandlePostAsync(HttpListenerRequest req, HttpListenerResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandlePutAsync(HttpListenerRequest req, HttpListenerResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandleGetAsync(HttpListenerRequest req, HttpListenerResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandleDeleteAsync(HttpListenerRequest req, HttpListenerResponse res) =>
            SendDefaultResponse(res);

        internal virtual Task HandleAnyAsync(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod == HttpMethod.Post.ToString())
                return HandlePostAsync(ctx.Request, ctx.Response);

            if (ctx.Request.HttpMethod == HttpMethod.Put.ToString())
                return HandlePutAsync(ctx.Request, ctx.Response);

            if (ctx.Request.HttpMethod == HttpMethod.Get.ToString())
                return HandleGetAsync(ctx.Request, ctx.Response);

            if (ctx.Request.HttpMethod == HttpMethod.Delete.ToString())
                return HandleDeleteAsync(ctx.Request, ctx.Response);


            return SendDefaultResponse(ctx.Response);
        }

        protected static Task SendDefaultResponse(HttpListenerResponse res)
        {
            return res.AnswerWithStatusCodeAsync(HttpStatusCode.NotImplemented);
        }
    }
}
