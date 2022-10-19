using Cherry.Extensions;
using Cherry.Routing;

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;

namespace Cherry.ConsoleApp
{
    public class Program
    {
        static async Task Main()
        {
            var server = new HttpServer(NullLogger<HttpServer>.Instance, "http://localhost:8081/")
                .RegisterController<HelloWorldController>()
                .RegisterController("/api/v2", async (ctx) =>
                {
                    await ctx.Response.AnswerWithStatusCodeAsync(
                        "API v2 says hello!",
                        HttpStatusCode.OK);
                });

            await server.RunAsync();
        }
    }

    [Route("/api/v1/helloworld")]
    public class HelloWorldController : HttpController
    {
        public override Task HandleGetAsync(HttpListenerContext ctx)
        {
            return ctx.Response.AnswerWithStatusCodeAsync(
                "Hello World!",
                HttpStatusCode.OK);
        }
    }
}