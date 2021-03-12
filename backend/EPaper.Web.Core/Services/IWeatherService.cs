using EPaper.Web.Core.Models;
using System.Threading.Tasks;

namespace EPaper.Web.Core.Services
{
    public interface IWeatherService
    {
        Task<WeatherForecast> GetWeatherForecast();
    }
}
