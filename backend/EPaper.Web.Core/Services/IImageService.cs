using System.Collections.Generic;
using System.Drawing;
using EPaper.Web.Core.Models;
using EPaper.Web.Core.Models.Connections;

namespace EPaper.Web.Core.Services
{
    using Models.Desk;

    public interface IImageService
    {
        Image CreateImageFromWeather(WeatherForecast weather);
        Image CreateImageFromConnections(IList<Connection> connections);
        
        Image GetImageFromWeatherIconUrl(string iconUrl, int width = 180, int height = 180);
        
        Image CreateImageFromDeskModel(DeskModel deskModel);
    }
}