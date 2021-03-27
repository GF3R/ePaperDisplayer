using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EPaper.Web.Core.Models;
using EPaper.Web.Core.Models.Configurations;
using Newtonsoft.Json.Linq;

namespace EPaper.Web.Core.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly TypeCodeConfiguration _configuration;

        private static string weatherUrl = "https://api.srgssr.ch/";

        private static string authUrl = "https://api.srgssr.ch/oauth/v1/accesstoken";

        private static string lat = "46.9559";

        private static string lon = "7.4907";

        private string query = "srf-meteo/forecast/{0},{1}";
        private static string clientId = "Rg5A8lPFw2qfYVOw0UblixYeSywUbAHR";
        private static string clientSecret = "PfbMm35WQWLUl14a";

        public WeatherService(TypeCodeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<WeatherForecast> GetWeatherForecast()
        {
            using HttpClient weatherClient = new HttpClient
            {
                BaseAddress = new Uri(weatherUrl)
            };

            var token = await Authenticate();
            weatherClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var weather = weatherClient.GetAsync(string.Format(query, lat, lon)).Result;
            var weatherString = weather.Content.ReadAsStringAsync().Result;
            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(weatherString);
            return ToWeather(weatherData);
        }

        private async Task<string> Authenticate()
        {
            using HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(authUrl)
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "?grant_type=client_credentials");

            var bytes = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic json = JToken.Parse(responseString);
            return json.access_token.ToString();
        }

        private WeatherForecast ToWeather(WeatherResponse response)
        {
            var hourlyForecast = response.forecast.SixtyMinutes;
            hourlyForecast.Sort((x, y) => DateTime.Compare(x.local_date_time, y.local_date_time));
            var now = DateTime.Now;
            return new WeatherForecast
            {
                Now = hourlyForecast.Where(item => item.local_date_time.Hour == now.Hour).Select(ForecastToHourlyWeather).First(),
                InOneHoursWeather = hourlyForecast.Where(item => item.local_date_time.Hour == now.AddHours(1).Hour).Select(ForecastToHourlyWeather).First(),
                InTwoHoursWeather = hourlyForecast.Where(item => item.local_date_time.Hour == now.AddHours(2).Hour).Select(ForecastToHourlyWeather).First(),
                InThreeHoursWeather = hourlyForecast.Where(item => item.local_date_time.Hour == now.AddHours(3).Hour).Select(ForecastToHourlyWeather).First(),
                TodaysWeather = response.forecast.day.Where(item => item.local_date_time.Date == now.Date).Select(ForecastToDailyWeather).First(),
                TomorrowsWeather = response.forecast.day.Where(item => item.local_date_time.Date == now.AddDays(1).Date).Select(ForecastToDailyWeather).First(),
            };
        }

        private HourlyWeather ForecastToHourlyWeather(HourlyForecast forecast)
        {
            var typecode = _configuration.WeatherTypeCodes.First(code => code.Code == forecast.SYMBOL_CODE);
            return new HourlyWeather
            {
                DateTime = forecast.local_date_time,
                Description = typecode.Description,
                ImageUrl = $"{_configuration.BaseUrl}{typecode.File}",
                CurrTemp = forecast.cur_color.temperature,
            };
        }

        private DailyWeather ForecastToDailyWeather(DailyForecast forecast)
        {
            var typecode = _configuration.WeatherTypeCodes.First(code => code.Code == forecast.SYMBOL_CODE);
            return new DailyWeather
            {
                DateTime = forecast.local_date_time,
                Description = typecode.Description,
                ImageUrl = $"{_configuration.BaseUrl}{typecode.File}",
                MinTemp = forecast.min_color.temperature,
                MaxTemp = forecast.max_color.temperature,
            };
        }

    }

}