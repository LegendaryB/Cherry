using Cherry.Extensions;
using Cherry.Middleware;
using Cherry.Routing;

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;
using System.Net.Mime;

namespace Cherry.ConsoleApp
{
    public class User
    {
        public string Name => "Daniel";
        public string Mail => "daniel@github.com";
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

    public class HelloWorldController : HttpController
    {
        public override Task HandleGetAsync(HttpListenerRequest req, HttpListenerResponse res)
        {
            return res.AnswerWithStatusCodeAsync(
                "Hello World!",
                HttpStatusCode.OK);
        }
    }

    [Route("/api/v1/users")]
    public class UserController : HttpController
    {
        public override Task HandleGetAsync(HttpListenerRequest req, HttpListenerResponse res)
        {
            return base.HandleGetAsync(req, res);
        }
    }

    public class Program
    {
        static async Task Main()
        {
            var server = new HttpServer(NullLogger<HttpServer>.Instance, "http://localhost:8081/")
                .RegisterMiddleware<JsonContentTypeMiddleware>()
                .RegisterController<HelloWorldController>("/api/v1/hello")
                .RegisterController<UserController>();

            await server.RunAsync();
        }
    }
}