namespace Cherry.ConsoleApp.Standalone
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new HttpServer("http://localhost:8081/")
                    .RegisterController<HelloWorldController>("/api/v1/helloWorld");

            await server.RunAsync();
        }
    }
}