using Cherry.Extensions;

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;

namespace Cherry.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new HttpServer(
                NullLogger<HttpServer>.Instance,
                "http://localhost:8081/");

            server.RegisterHandlerForRoute("/api/test", async (ctx) =>
            {
                await ctx.Response.AnswerWithStatusCodeAsync(
                    "Hello handler",
                    HttpStatusCode.OK);
            });

            await server.RunAsync();
    }
}