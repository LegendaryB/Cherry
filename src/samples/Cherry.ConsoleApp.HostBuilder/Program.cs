using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Net;

namespace Cherry.ConsoleApp.HostBuilder
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddControllers();

                    services.AddSingleton((serviceProvider) =>
                    {
                        var controllers = serviceProvider.GetService<IEnumerable<HttpController>>();

                        return new HttpServer(() =>
                        {
                            var listener = new HttpListener();
                            listener.Prefixes.Add("http://localhost:8081/");

                            return listener;
                        });
                    });

                    services.AddHostedService<App>();
                })
                .Build()
                .RunAsync();
        }
    }
}