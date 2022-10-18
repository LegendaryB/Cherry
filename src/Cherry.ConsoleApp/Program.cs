using Cherry.Extensions;
using Cherry.Routing;

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;

namespace Cherry.ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var server = new HttpServer(NullLogger<HttpServer>.Instance, "http://localhost:8081/");
            server.RegisterController<HelloWorldController>("/api");
            server.AutoRegisterControllers();

            // server.AutoRegisterControllers();

            //server.RegisterHandlerForRoute("/api/test", async (ctx) =>
            //{
            //    await ctx.Response.AnswerWithStatusCodeAsync(
            //        "Hello handler",
            //        HttpStatusCode.OK);
            //});

            await server.RunAsync();
        }
    }

    [Route("/api/v1/helloWorld")]
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