using EPaper.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System.Threading;
using System.Threading.Tasks;
using EPaper.Web.Core.Models.Configurations;
using EPaper.Web.Core.Services;


namespace EPaper.Web.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController : Controller
    {
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly ImageConfiguration _imageConfiguration;
        private readonly IWeatherService _weatherService;
        private readonly IMqttClient _mqttClient;

        public ImageController(MqttConfiguration mqttConfiguration, ImageConfiguration imageConfiguration, IWeatherService weatherService)
        {
            _mqttConfiguration = mqttConfiguration;
            _imageConfiguration = imageConfiguration;
            _weatherService = weatherService;
            this._mqttClient = new MqttFactory().CreateMqttClient();
        }

        [HttpPost]
        public async Task<IActionResult> FromUrlAsBytes(string imageUrl, int width, int height, int threshold = 128)
        {
            var epaperImage = new EPaperImage(imageUrl, width, height, threshold);
            await this.Publish(epaperImage.GetEPaperBytes());
            return Ok(epaperImage);
        }

        [HttpPost]
        public async Task<MqttClientPublishResult> Clear()
        {
            return await Publish(new byte[0]);
        }

        [HttpGet]
        public async Task<WeatherForecast> Weather()
        {
            return await _weatherService.GetWeatherForecast();
        }

        [HttpGet]
        public async Task<IActionResult> WeatherImage()
        {
            var service = new ImageService();
            var weather = await this.Weather();
            var image = service.CreateImageFromWeather(weather);
            var epaperImage = new EPaperImage(image, _imageConfiguration.Width, _imageConfiguration.Height) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
            await this.Publish(epaperImage.GetEPaperBytes());
            return Ok(epaperImage);
        }


        private async Task Connect()
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(_mqttConfiguration.ClientId)
                .WithTcpServer(_mqttConfiguration.Server, _mqttConfiguration.Port)
                .WithCredentials(_mqttConfiguration.Username, _mqttConfiguration.Password)
                .Build();
            await _mqttClient.ConnectAsync(options, CancellationToken.None);
        }

        private async Task<MqttClientPublishResult> Publish(byte[] bytes)
        {
            await this.Connect();
            return await _mqttClient.PublishAsync(new MqttApplicationMessage()
            {
                Topic = _mqttConfiguration.Topic,
                Payload = bytes,
                Retain = true
            });
        }

    }
}
