using Cherry.Extensions;

using System.Net;

namespace Cherry.Routing
{
    public class HttpController
    {
        protected HttpController() { }

        public virtual Task HandlePostAsync(HttpRequest req, HttpResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandlePutAsync(HttpRequest req, HttpResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandleGetAsync(HttpRequest req, HttpResponse res) =>
            SendDefaultResponse(res);

        public virtual Task HandleDeleteAsync(HttpRequest req, HttpResponse res) =>
            SendDefaultResponse(res);

        internal virtual Task HandleAnyAsync(HttpRequest req, HttpResponse res)
        {
            if (req.Method == HttpMethod.Post.ToString())
                return HandlePostAsync(req, res);

            if (req.Method == HttpMethod.Put.ToString())
                return HandlePutAsync(req, res);

            if (req.Method == HttpMethod.Get.ToString())
                return HandleGetAsync(req, res);

            if (req.Method == HttpMethod.Delete.ToString())
                return HandleDeleteAsync(req, res);

            return SendDefaultResponse(res);
        }

        protected static Task SendDefaultResponse(HttpResponse res)
        {
            return Task.CompletedTask;
            // return res.AnswerWithStatusCodeAsync(HttpStatusCode.NotImplemented);
        }
    }
}
