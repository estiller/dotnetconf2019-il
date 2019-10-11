using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherService.Model;
using WeatherService.Workers.Model;

namespace WeatherService.Workers
{
    public class WeatherWorker : BackgroundService
    {
        private readonly ILogger<WeatherWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public WeatherWorker(ILogger<WeatherWorker> logger,
                            IConfiguration configuration,
                            IHttpClientFactory httpClientFactory,
                            IMemoryCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var accessToken = _configuration["AccuWeatherToken"];
                    (Weather weather, TimeSpan timeToWait) = string.IsNullOrWhiteSpace(accessToken) ?
                        GetDummyWeather() :
                        await GetWeatherFromServiceAsync(accessToken, stoppingToken);

                    _cache.Set("LatestWeather", weather);
                    await Task.Delay(timeToWait, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error fetching weather data");
                }
            }
        }

        private static (Weather weather, TimeSpan timeToWait) GetDummyWeather()
        {
            var weather = new Weather("Dummy Location, Israel", DateTimeOffset.Now, "Clear",
                "https://developer.accuweather.com/sites/default/files/01-s.png", 20, 25, 15);
            var timeToWait = TimeSpan.FromMinutes(1);
            return (weather, timeToWait);
        }

        private async Task<(Weather weather, TimeSpan timeToWait)> GetWeatherFromServiceAsync(string accessToken, CancellationToken stoppingToken)
        {
            var client = _httpClientFactory.CreateClient("AccuWeather");
            var response = await client.GetAsync($"http://dataservice.accuweather.com/currentconditions/v1/215854?apikey={accessToken}&details=true", stoppingToken);
            response.EnsureSuccessStatusCode();

            var model = await JsonSerializer.DeserializeAsync<Forecast[]>(await response.Content.ReadAsStreamAsync(), cancellationToken: stoppingToken);
            var weather = ToWeather(model.First());

            DateTimeOffset? expiresHeader = response.Content.Headers.Expires;
            TimeSpan timeToWait = expiresHeader?.UtcDateTime.Subtract(DateTime.UtcNow) ?? TimeSpan.FromMinutes(60);

            return (weather, timeToWait);
        }

        private static Weather ToWeather(Forecast forecast)
        {
            return new Weather(
                "Tel-Aviv, Israel",
                forecast.LocalObservationDateTime,
                forecast.WeatherText,
                $"https://developer.accuweather.com/sites/default/files/{forecast.WeatherIcon:D2}-s.png",
                forecast.Temperature.Metric.Value,
                forecast.TemperatureSummary.Past24HourRange.Maximum.Metric.Value,
                forecast.TemperatureSummary.Past24HourRange.Minimum.Metric.Value);
        }
    }
}