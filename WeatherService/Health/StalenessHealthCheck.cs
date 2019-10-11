using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WeatherService.Model;

namespace WeatherService.Health
{
    public class StalenessHealthCheck : IHealthCheck
    {
        private readonly IMemoryCache _cache;

        public StalenessHealthCheck(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var weather = _cache.Get<Weather>("LatestWeather");
            if (weather == null)
            {
                return Task.FromResult(HealthCheckResult.Degraded("No weather report available"));
            }

            var timeSinceLastReport = DateTimeOffset.Now - weather.LocalObservationDateTime;
            if (timeSinceLastReport > TimeSpan.FromMinutes(60))
            {
                return Task.FromResult(HealthCheckResult.Degraded($"Last available report is from {weather.LocalObservationDateTime}"));
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}