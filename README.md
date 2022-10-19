<div align="center">

<h1>Cherry</h1>

[![forthebadge](https://forthebadge.com/images/badges/fuck-it-ship-it.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)

[![GitHub license](https://img.shields.io/github/license/legendaryb/cherry.svg?longCache=true&style=flat-square)](https://github.com/LegendaryB/Cherry/blob/main/LICENSE.txt)

A self-hostable application to download files in a folder from Google Drive powered by Go.
</div><br>

## 🎯 Features
* Build around .NET's [HttpListener](https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-7.0)
* Setup of the `HttpServer` instance via a fluent api
* Lightweight. The only dependency is `Microsoft.Extensions.Logging.Abstractions` to make logging of internal events possible
* Built-in `HttpRouter` is interchangeable by implementing the interface `IHttpRouter` and calling `HttpServer.Use<MyRouter>()`
* HTTP Controller support (limited to `POST`, `GET`, `PUT`, `DELETE`, but extendable)
    * Use `Func<THttpRequest, THttpRespoonse, Task>` to implement controllers
* Middleware support
    * Use `Func<THttpContext, Task>` to implement custom middlewares

## 📝 Usage

```csharp
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
                .RegisterMiddleware<JsonContentTypeMiddleware>() // installs a global middleware because no specific route was provided
                .RegisterController<HelloWorldController>("/api/v1/hello") // Path must be provided because the class is not decorated with the route attribute
                .RegisterController<UserController>(); // No path must be provided, because the class is decorated with the Route attribute

            // server.AutoRegisterControllers(); // Can be used to automatically discover controllers from a given assembly. Uses reflection and ignores all types which are not decorated with the Route attribute and inheriting from the HttpController base class

            await server.RunAsync();
        }
    }
}
```