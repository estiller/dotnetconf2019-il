using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using WeatherService.Model;

namespace WeatherService.Services
{
    public class WeatherGrpcService : WeatherService.WeatherServiceBase
    {
        private readonly IMemoryCache _cache;

        public WeatherGrpcService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public override Task<WeatherResponse> GetWeather(Empty request, ServerCallContext context)
        {
            var weather = _cache.Get<Weather>("LatestWeather");
            return Task.FromResult(ToWeatherResponse(weather));
        }


        public override async Task GetWeatherStream(Empty request, IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var weather = _cache.Get<Weather>("LatestWeather");
                await responseStream.WriteAsync(ToWeatherResponse(weather));
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        private static WeatherResponse ToWeatherResponse(Weather weather)
        {
            var weatherResponse = new WeatherResponse
            {
                Location = weather.Location,
                LocalObservationDateTime = Timestamp.FromDateTimeOffset(weather.LocalObservationDateTime),
                WeatherText = weather.WeatherText,
                WeatherIconUrl = weather.WeatherIconUrl,
                Temperature = weather.Temperature,
                PastDayMaxTemperature = weather.PastDayMaxTemperature,
                PastDayMinTemperature = weather.PastDayMinTemperature
            };
            return weatherResponse;
        }
    }
}