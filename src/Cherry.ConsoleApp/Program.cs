using Cherry.Extensions;
using Cherry.Middleware;
using Cherry.Routing;

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Cherry.ConsoleApp
{
    public class Person
    {
        public string Name => "Daniel";
        public string Mail => "daniel@github.com";
        public int Age => 28;
    }

    public class Program
    {
        static async Task Main()
        {
            var server = new HttpServer(NullLogger<HttpServer>.Instance, "http://localhost:8081/")
                .RegisterMiddleware<JsonContentTypeMiddleware>()
                .RegisterController<HelloWorldController>()
                .RegisterController("/api/v2", async (ctx) =>
                {
                    await ctx.Response.AnswerWithStatusCodeAsync(
                        JsonSerializer.Serialize(new Person(), new JsonSerializerOptions { WriteIndented = true }),
                        HttpStatusCode.OK);
                });

            await server.RunAsync();
        }
    }

    public class JsonContentTypeMiddleware : IMiddleware
    {
        public Task HandleRequestAsync(
            HttpListenerRequest req,
            HttpListenerResponse res)
        {
            res.ContentType = MediaTypeNames.Application.Json;

            return Task.CompletedTask;
        }
    }

    [Route("/api/v1/helloworld")]
    public class HelloWorldController : HttpController
    {
        public override Task HandleGetAsync(HttpListenerRequest req, HttpListenerResponse res)
        {
            return res.AnswerWithStatusCodeAsync(
                "Hello World!",
                HttpStatusCode.OK);
        }
    }
}