using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeatherService.Model;

namespace WeatherService.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly IMemoryCache _cache;

        public WeatherController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public ActionResult<Weather> GetWeatherForecast()
        {
            return _cache.Get<Weather>("LatestWeather");
        }
    }
}
