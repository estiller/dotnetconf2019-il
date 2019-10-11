using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MonitorService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var url = _configuration["ServiceUrl"];
                    var client = new HttpClient { BaseAddress = new Uri(url) };
                    var response = await client.GetAsync("health", stoppingToken);
                    var content = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation($"Health status code: {response.StatusCode}. Response: {content}");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An unexpected exception occured");
                }
            }
        }
    }
}
