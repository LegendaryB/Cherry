using Cherry.Middleware;
using Cherry.Routing;

using Microsoft.Extensions.Logging.Abstractions;

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
            HttpRequest req,
            HttpResponse res)
        {
            req.Body = null;

            // res.ContentType = MediaTypeNames.Application.Json;
            return Task.CompletedTask;
        }
    }

    public class HelloWorldController : HttpController
    {
        public override Task HandleGetAsync(HttpRequest req, HttpResponse res)
        {
            //return res.AnswerWithStatusCodeAsync(
            //    "Hello World!",
            //    HttpStatusCode.OK);
            return base.HandleGetAsync(
                req,
                res);
        }
    }

    [Route("/api/v1/users")]
    public class UserController : HttpController
    {
        public override Task HandleGetAsync(HttpRequest req, HttpResponse res)
        {
            return base.HandleGetAsync(
                req,
                res);
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