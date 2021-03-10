﻿using EPaper.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System.Threading;
using System.Threading.Tasks;
using EPaper.Web.Core.Services;


namespace EPaper.Web.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController : Controller
    {
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly IMqttClient _mqttClient;

        public ImageController(MqttConfiguration mqttConfiguration)
        {
            _mqttConfiguration = mqttConfiguration;
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
        public async Task<WeatherResponse> Weather()
        {
            var service = new WeatherService();
            return await service.GetWeatherData();
        }

        [HttpGet]
        public async Task<IActionResult> WeatherImage()
        {
            var service = new ImageService();
            var weather = await this.Weather();
            var image = service.CreateImageFromWeather(weather);
            var epaperImage = new EPaperImage(image, 400, 300);
            epaperImage.WhiteToBlackThrehshold = 250;
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
            });
        }

    }
}