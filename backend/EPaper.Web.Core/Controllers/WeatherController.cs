namespace EPaper.Web.Core.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Configurations;
    using Services;

    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherController : Controller
    {
        private readonly IMqttService _mqttService;
        private readonly ImageConfiguration _imageConfiguration;
        private readonly TypeCodeConfiguration _typeCodeConfiguration;
        private readonly IWeatherService _weatherService;
        private readonly IImageService _imageService;

        public WeatherController(IMqttService mqttService, ImageConfiguration imageConfiguration, TypeCodeConfiguration typeCodeConfiguration, IWeatherService weatherService, IImageService imageService)
        {
            _mqttService = mqttService;
            _imageConfiguration = imageConfiguration;
            _typeCodeConfiguration = typeCodeConfiguration;
            _weatherService = weatherService;
            _imageService = imageService;

        }
        [HttpGet]
        public async Task<IActionResult> WeatherImage()
        {
            var weather = await _weatherService.GetWeatherForecast();
            var image = this._imageService.CreateImageFromWeather(weather);
            var epaperImage = new EPaperImage(image, _imageConfiguration.Width, _imageConfiguration.Height) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
            await this._mqttService.Publish(epaperImage.GetEPaperBytes());
            return Ok(epaperImage);
        }

        [HttpGet]
        public async Task DisplayAllimages()
        {
            var baseUrl = _typeCodeConfiguration.BaseUrl;
            foreach (var typeCode in _typeCodeConfiguration.WeatherTypeCodes)
            {
                var image = this._imageService.GetImageFromWeatherIconUrl($"{baseUrl}{typeCode.File}", 400, 300);
                var epaperImage = new EPaperImage(image, _imageConfiguration.Width, _imageConfiguration.Height) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
                await this._mqttService.Publish(epaperImage.GetEPaperBytes());
                Thread.Sleep(1500);
            }
        }
    }
}
