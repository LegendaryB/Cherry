using System.Net;

namespace Cherry
{
    public class HttpResponse
    {
        public HttpListenerResponse Response { get; }

        public HttpResponse(HttpListenerResponse res)
        {
            Response = res;
        }
    }
}