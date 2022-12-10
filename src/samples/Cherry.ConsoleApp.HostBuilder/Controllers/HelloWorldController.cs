using System.Net;

namespace Cherry.ConsoleApp.HostBuilder
{
    [Route("/api/v1/helloWorld")]
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