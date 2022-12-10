using System.Net;

namespace Cherry.ConsoleApp.Standalone
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new HttpServer(() =>
            {
                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:8081/");

                return listener;
            })
            .RegisterController<HelloWorldController>("/api/v1/helloWorld");

            await server.RunAsync();
        }
    }
}