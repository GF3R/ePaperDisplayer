using System.Drawing;
using EPaper.Web.Core.Models;

namespace EPaper.Web.Core.Services
{
    public interface IImageService
    {
        Image CreateImageFromWeather(WeatherForecast weather);
        Image GetImageFromWeatherIconUrl(string iconUrl, int width = 180, int height = 180);
    }
}