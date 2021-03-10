using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EPaper.Web.Core.Models;

namespace EPaper.Web.Core.Services
{
    public class WeatherService
    {
        private static string openWeatherAPIKey = "b7fa6b6a949ce5f9d6cf2af10bf2e552";

        private static string baseUrl = "https://api.openweathermap.org/data/2.5/";

        private static string lat = "46.956170";

        private static string lon = "7.482840";

        private string query = "onecall?lat={0}&lon={1}&exclude=hourly,minutely&appid={2}";


        public async Task<WeatherResponse> GetWeatherData()
        {
            using HttpClient client = new HttpClient{
                BaseAddress = new Uri(baseUrl)
            };
            var url = string.Format(query, lat, lon, openWeatherAPIKey);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var response = await client.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(json, options);
            return weatherData;
        }
    }
}