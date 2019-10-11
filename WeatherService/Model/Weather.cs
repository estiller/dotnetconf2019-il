using System;

namespace WeatherService.Model
{
    public class Weather
    {
        public Weather(string location, DateTimeOffset localObservationDateTime, string weatherText, string weatherIconUrl,
            float temperature, float pastDayMaxTemperature, float pastDayMinTemperature)
        {
            Location = location;
            LocalObservationDateTime = localObservationDateTime;
            WeatherText = weatherText;
            WeatherIconUrl = weatherIconUrl;
            Temperature = temperature;
            PastDayMaxTemperature = pastDayMaxTemperature;
            PastDayMinTemperature = pastDayMinTemperature;
        }

        public string Location { get; }
        public DateTimeOffset LocalObservationDateTime { get; }
        public string WeatherText { get; }
        public string WeatherIconUrl { get; }
        public float Temperature { get; }
        public float PastDayMaxTemperature { get; }
        public float PastDayMinTemperature { get; }
    }
}