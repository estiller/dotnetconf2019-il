using System;

// ReSharper disable UnusedMember.Global
namespace WeatherService.Workers.Model
{
    public class Forecast
    {
        public DateTimeOffset LocalObservationDateTime { get; set; }
        public string WeatherText { get; set; } = default!;
        public int WeatherIcon { get; set; }
        public Measurements Temperature { get; set; } = default!;
        public MeasurementsSummary TemperatureSummary { get; set; } = default!;
    }

    public class Measurements
    {
        public Measurement Metric { get; set; } = default!;
        public Measurement Imperial { get; set; } = default!;
    }

    public class Measurement
    {
        public float Value { get; set; }
        public string Unit { get; set; } = default!;
        public int UnitType { get; set; }
    }


    public class MeasurementsSummary
    {
        public MeasurementsRange Past6HourRange { get; set; } = default!;
        public MeasurementsRange Past12HourRange { get; set; } = default!;
        public MeasurementsRange Past24HourRange { get; set; } = default!;
    }

    public class MeasurementsRange
    {
        public Measurements Minimum { get; set; } = default!;
        public Measurements Maximum { get; set; } = default!;
    }
}