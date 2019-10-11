using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace WeatherService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, options) =>
                    {
                        if (context.HostingEnvironment.IsProduction())
                        {
                            // Disable Secured GRPC in production, as we will be running behind an ingress controller
                            options.Listen(IPAddress.Any, 80, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
