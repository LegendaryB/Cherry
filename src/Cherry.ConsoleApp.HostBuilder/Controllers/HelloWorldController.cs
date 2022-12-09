using Cherry.Controller;
using Cherry.Extensions;

using System.Net;

namespace Cherry.ConsoleApp.HostBuilder
{
    public class HelloWorldController : HttpController
    {
        public override Task GetAsync(HttpRequest req, HttpResponse res)
        {
            return res.AnswerWithStatusCodeAsync(
                "Hello World!",
                HttpStatusCode.OK);
        }
    }
}