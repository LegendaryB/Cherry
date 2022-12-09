using Microsoft.Extensions.Hosting;

namespace Cherry.ConsoleApp.HostBuilder
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {

                })
                .Build()
                .RunAsync();
        }
    }
}