using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace WeatherClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new WeatherService.Services.WeatherService.WeatherServiceClient(channel);
            var responseStream = client.GetWeatherStream(new Empty()).ResponseStream;
            await foreach (var response in responseStream.ReadAllAsync())
            {
                Location.Text = response.Location;
                Timestamp.Text = response.LocalObservationDateTime.ToDateTimeOffset().ToLocalTime().ToString();
                WeatherText.Text = response.WeatherText;
                WeatherIcon.Source = new BitmapImage(new Uri(response.WeatherIconUrl));
                Temperature.Text = $"{response.Temperature:##} ℃";
                TemperatureMax.Text = $"{response.PastDayMaxTemperature:##} ℃";
                TemperatureMin.Text = $"{response.PastDayMinTemperature:##} ℃";
            }
        }
    }
}
