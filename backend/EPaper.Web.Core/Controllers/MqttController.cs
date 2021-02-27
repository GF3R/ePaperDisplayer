using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EPaper.Web.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MqttController : Controller
    {

        private readonly ILogger<MqttController> _logger;
        private IMqttClient mqttClient;

        public MqttController(ILogger<MqttController> logger)
        {
            var factory = new MqttFactory();
            this.mqttClient = factory.CreateMqttClient();
        }
        
        private async Task<MqttClientAuthenticateResult> Connect()
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId("Client1")
                .WithTcpServer("farmer.cloudmqtt.com", 11140)
                .WithCredentials("prkyqurh", "Prc1iqpPQdYE")
                .Build();
            return await mqttClient.ConnectAsync(options, CancellationToken.None);
        }

        [HttpPost]
        public async Task<Task<MqttClientPublishResult>> FromImage(IFormFile image)
        {
                var converter = new ImageConverter();
                return this.Publish((byte[]) converter.ConvertTo(image, typeof(byte[])));
        }

        [HttpPost]
        public void FromUrl(string imageUrl)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(imageUrl);
                    Publish(bytes: data);
                }
            }
            catch (Exception e)
            {

            }
        }

        [HttpPost]
        public void Clear()
        {
            Publish(new byte[0]);
        }

        private async Task<MqttClientPublishResult> Publish(byte[] bytes)
        {
            await this.Connect();
            return await mqttClient.PublishAsync(new MqttApplicationMessage()
            {
                Topic = "epaper/EPAPER-001/image",
                Payload = bytes
            });
        }

        [HttpGet]
        public async Task<MqttClientPublishResult> Test()
        {
            await this.Connect();

            return await mqttClient.PublishAsync(new MqttApplicationMessage()
            {
                Topic = "epaper/EPAPER-001/image",
                Payload = new byte[]
                {
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XF0, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XC0, 0X1F, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0X80, 0X07, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0X00, 0X01, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFC, 0X01, 0X80, 0X3F, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFC, 0X01, 0XE0, 0X1F, 0XFF, 0XFF, 0XFF, 0XFF, 0XF8, 0X10, 0XF8, 0X0F, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XF8, 0X70, 0XF8, 0X0F, 0XFF, 0XFF, 0XFF, 0XFF, 0XF0, 0X70, 0XF8, 0X0F, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XF0, 0XF0, 0XF8, 0X07, 0XFF, 0XFF, 0XFF, 0XFF, 0XE1, 0XF0, 0XFC, 0X07, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XE1, 0XF0, 0XFC, 0X03, 0XFF, 0XFF, 0XFF, 0XFF, 0XE3, 0XF0, 0XFC, 0X01, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XE3, 0XF0, 0XFC, 0X21, 0XFF, 0XFF, 0XFF, 0XFF, 0XC3, 0XF8, 0XFE, 0X21, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XC3, 0XF8, 0XFE, 0X10, 0XFF, 0XFF, 0XFF, 0XFF, 0XC7, 0XF8, 0XFE, 0X10, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XC7, 0XF8, 0XFE, 0X10, 0XFF, 0XFF, 0XFF, 0XFF, 0XC7, 0XF8, 0XFF, 0X18, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XC7, 0XF8, 0XFF, 0X08, 0XFF, 0XFF, 0XFF, 0XFF, 0XC7, 0XF0, 0XFF, 0X00, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XC7, 0XF0, 0XFF, 0X80, 0XFF, 0XFF, 0XFF, 0XFF, 0XC7, 0XF0, 0XFF, 0X80, 0X7F, 0XFF,
                    0XFF, 0XFF, 0XC7, 0XF0, 0XFF, 0XC0, 0X3F, 0XFF, 0XFF, 0XFF, 0X87, 0XF1, 0XFF, 0XC0, 0X3F, 0XFF,
                    0XFF, 0XFF, 0X07, 0XF1, 0XFF, 0XE0, 0X1F, 0XFF, 0XFF, 0XFF, 0X03, 0XE1, 0XFF, 0XE0, 0X1F, 0XFF,
                    0XFF, 0XFE, 0X01, 0XE1, 0XFF, 0XF1, 0X1F, 0XFF, 0XFF, 0XFC, 0X01, 0XE1, 0XFF, 0XF1, 0X1F, 0XFF,
                    0XFF, 0XF8, 0X21, 0XE1, 0XFF, 0XF1, 0X1F, 0XFF, 0XFF, 0XE0, 0X31, 0XE1, 0XFF, 0XF1, 0X1F, 0XFF,
                    0XFF, 0XE0, 0XF1, 0XF1, 0XFF, 0XF1, 0X1F, 0XFF, 0XFF, 0XC1, 0XF1, 0XF0, 0XFF, 0XF1, 0X0F, 0XFF,
                    0XFF, 0X83, 0XF1, 0XF0, 0XFF, 0XF1, 0X0F, 0XFF, 0XFF, 0X87, 0XF1, 0XF0, 0XFF, 0XF1, 0X0F, 0XFF,
                    0XFF, 0X07, 0XF1, 0XF0, 0XFF, 0XF0, 0X8F, 0XFF, 0XFF, 0X0F, 0XF1, 0XF0, 0XFF, 0XF0, 0X8F, 0XFF,
                    0XFE, 0X0F, 0XF1, 0XF0, 0XFF, 0XF8, 0X8F, 0XFF, 0XFE, 0X1F, 0XF1, 0XF0, 0XFF, 0XF8, 0X07, 0XFF,
                    0XFC, 0X1F, 0XE1, 0XF0, 0XFF, 0XF8, 0X07, 0XFF, 0XFC, 0X1F, 0XE1, 0XF8, 0XFF, 0XFC, 0X03, 0XFF,
                    0XFC, 0X1F, 0XE1, 0XF8, 0XFF, 0XFC, 0X03, 0XFF, 0XFC, 0X07, 0XE3, 0XF8, 0XFF, 0XFC, 0X23, 0XFF,
                    0XF8, 0X01, 0XE3, 0XF8, 0XFF, 0XFE, 0X21, 0XFF, 0XF8, 0X00, 0XE3, 0XF8, 0XFF, 0XFE, 0X21, 0XFF,
                    0XF8, 0X43, 0X23, 0XF8, 0XFF, 0XFE, 0X31, 0XFF, 0XF8, 0X40, 0X03, 0XF8, 0XFF, 0XFE, 0X31, 0XFF,
                    0XF0, 0X60, 0X03, 0XF8, 0XFF, 0XFE, 0X31, 0XFF, 0XF0, 0XE0, 0X03, 0XF8, 0XFF, 0XFE, 0X31, 0XFF,
                    0XE0, 0XFC, 0X03, 0XF8, 0X7F, 0XFE, 0X30, 0XFF, 0XE1, 0XFF, 0X83, 0XF8, 0X7F, 0XFE, 0X30, 0XFF,
                    0XE1, 0XFF, 0XC3, 0XFC, 0X7F, 0XFE, 0X30, 0XFF, 0XE1, 0XFF, 0XC3, 0XFC, 0X7F, 0XFE, 0X38, 0XFF,
                    0XE1, 0XFF, 0XE3, 0XFC, 0X3F, 0XFE, 0X38, 0X7F, 0XE1, 0XFF, 0XE3, 0XFC, 0X3F, 0XFE, 0X18, 0X7F,
                    0XE1, 0XFF, 0XE3, 0XFC, 0X3F, 0XFE, 0X18, 0X3F, 0XE1, 0XFF, 0XE3, 0XFC, 0X3F, 0XFE, 0X1C, 0X3F,
                    0XF1, 0XFF, 0XE3, 0XFE, 0X3F, 0XFF, 0X0C, 0X3F, 0XF1, 0XFF, 0XE3, 0XFE, 0X3F, 0XFF, 0X0E, 0X3F,
                    0XE1, 0XFF, 0XE3, 0XFE, 0X1F, 0XFF, 0X86, 0X3F, 0XE1, 0XFF, 0XE3, 0XFE, 0X1F, 0XFF, 0X86, 0X3F,
                    0XE1, 0XFF, 0XE3, 0XFE, 0X1F, 0XFF, 0X86, 0X3F, 0XE3, 0XFF, 0XE3, 0XFE, 0X1F, 0XFF, 0X86, 0X3F,
                    0XE3, 0XFF, 0XE3, 0XFF, 0X1F, 0XFF, 0X86, 0X3F, 0XC3, 0XFF, 0XE3, 0XFF, 0X1F, 0XFF, 0X8E, 0X3F,
                    0XC3, 0XFF, 0XE3, 0XFF, 0X1F, 0XFF, 0X8E, 0X3F, 0XC7, 0XFF, 0XC3, 0XFF, 0X1F, 0XFF, 0X8E, 0X3F,
                    0XC7, 0XFF, 0XC3, 0XFF, 0X0F, 0XFF, 0X8E, 0X3F, 0XC3, 0XFF, 0XC3, 0XFF, 0X0F, 0XFF, 0X8E, 0X3F,
                    0XC0, 0X7F, 0XC7, 0XFF, 0X0F, 0XFF, 0X8E, 0X1F, 0XC0, 0X3F, 0XC7, 0XFF, 0X0F, 0XFF, 0X8E, 0X0F,
                    0XE0, 0X0F, 0XC7, 0XFF, 0X0F, 0XFF, 0X86, 0X0F, 0XE2, 0X07, 0XC7, 0XFF, 0X0F, 0XFF, 0X87, 0X0F,
                    0XE3, 0X07, 0XC7, 0XFF, 0X8F, 0XFF, 0XC7, 0X8F, 0XE3, 0X83, 0X87, 0XFF, 0X8F, 0XFF, 0XC7, 0X8F,
                    0XE3, 0XC0, 0X87, 0XFF, 0X8F, 0XFF, 0XC7, 0X0F, 0XE3, 0XE0, 0X0F, 0XFF, 0X8F, 0XFF, 0XC7, 0X0F,
                    0XE3, 0XF0, 0X07, 0XFF, 0X87, 0XFF, 0XC7, 0X1F, 0XE3, 0XF8, 0X07, 0XFF, 0X87, 0XFF, 0XC7, 0X1F,
                    0XC3, 0XFC, 0X07, 0XFF, 0X87, 0XFF, 0XC7, 0X1F, 0XC3, 0XFE, 0X07, 0XFF, 0X87, 0XFF, 0XC3, 0X0F,
                    0XC3, 0XFE, 0X07, 0XFF, 0X87, 0XFF, 0XC3, 0X0F, 0XC7, 0XFF, 0X07, 0XFF, 0X87, 0XFF, 0XC3, 0X8F,
                    0XC1, 0XFF, 0XC7, 0XFF, 0X87, 0XFF, 0XE3, 0X87, 0XC0, 0X3F, 0X87, 0XFF, 0X87, 0XFF, 0XE3, 0X83,
                    0XC0, 0X1F, 0X87, 0XFF, 0X87, 0XFF, 0XE3, 0X83, 0XC0, 0X1F, 0X87, 0XFF, 0X87, 0XFF, 0XE3, 0XC3,
                    0XE2, 0X07, 0X87, 0XFF, 0X83, 0XFF, 0XE3, 0XC3, 0XE1, 0X07, 0X87, 0XFF, 0X83, 0XFF, 0XE3, 0XC3,
                    0XE1, 0X81, 0X87, 0XFF, 0X83, 0XFF, 0XE3, 0XC3, 0XE1, 0XC0, 0X87, 0XFF, 0XC3, 0XFF, 0XE3, 0XC7,
                    0XE1, 0XE0, 0X0F, 0XFF, 0XC3, 0XFF, 0XE3, 0XC7, 0XE1, 0XF0, 0X07, 0XFF, 0XC3, 0XFF, 0XE3, 0XC7,
                    0XE1, 0XF8, 0X0F, 0XFF, 0XC3, 0XFF, 0XE3, 0XC7, 0XE1, 0XFE, 0X0F, 0XFF, 0XC3, 0XFF, 0XE3, 0XC7,
                    0XF1, 0XFF, 0X0F, 0XFF, 0XC3, 0XFF, 0XE3, 0XC3, 0XF1, 0XFF, 0X0F, 0XFF, 0XE3, 0XFF, 0XE3, 0XC3,
                    0XF0, 0XFF, 0X0F, 0XFF, 0XE1, 0XFF, 0XE3, 0XE3, 0XF0, 0XFF, 0X8F, 0XFF, 0XE1, 0XFF, 0XE3, 0XE3,
                    0XF8, 0X7F, 0X8F, 0XFF, 0XE1, 0XFF, 0XE3, 0XE3, 0XF8, 0X3F, 0X8F, 0XFF, 0XE1, 0XFF, 0XE3, 0XE3,
                    0XFC, 0X1F, 0X87, 0XFF, 0XE1, 0XFF, 0XE3, 0XE3, 0XFE, 0X0F, 0X87, 0XFF, 0XE1, 0XFF, 0XE1, 0XE3,
                    0XFF, 0X0F, 0X87, 0XFF, 0XE1, 0XFF, 0XE1, 0XE3, 0XFF, 0X87, 0X87, 0XFF, 0XE1, 0XFF, 0XE1, 0XE3,
                    0XFF, 0X83, 0X87, 0XFF, 0XE1, 0XFF, 0XF1, 0XC3, 0XFF, 0X81, 0X87, 0XFF, 0XE1, 0XFF, 0XF1, 0XC3,
                    0XFF, 0XC0, 0X87, 0XFF, 0XE1, 0XFF, 0XF1, 0X83, 0XFF, 0XE0, 0X0F, 0XFF, 0XF1, 0XFF, 0XF1, 0X87,
                    0XFF, 0XF0, 0X0F, 0XFF, 0XF1, 0XFF, 0XF1, 0X87, 0XFF, 0XFC, 0X0F, 0XFF, 0XE0, 0XFF, 0XF1, 0X8F,
                    0XFF, 0XFE, 0X0F, 0XFF, 0XE0, 0XFF, 0XF1, 0X8F, 0XFF, 0XFE, 0X0F, 0XFF, 0XE0, 0XFF, 0XE1, 0X8F,
                    0XFF, 0XFF, 0X0F, 0XFF, 0XE0, 0XFF, 0XE1, 0X8F, 0XFF, 0XFF, 0X07, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F,
                    0XFF, 0XFF, 0X87, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F, 0XFF, 0XFF, 0X87, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F,
                    0XFF, 0XFF, 0XC3, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F, 0XFF, 0XFF, 0XC3, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F,
                    0XFF, 0XFF, 0XC3, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F, 0XFF, 0XFF, 0XE3, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F,
                    0XFF, 0XFF, 0XE3, 0XFF, 0XF0, 0XFF, 0XE3, 0X8F, 0XFF, 0XFF, 0XE1, 0XFF, 0XF0, 0XFF, 0XE3, 0X0F,
                    0XFF, 0XFF, 0XE1, 0XFF, 0XF0, 0XFF, 0XE3, 0X0F, 0XFF, 0XFF, 0XE1, 0XFF, 0XF0, 0XFF, 0XE3, 0X1F,
                    0XFF, 0XFF, 0XF1, 0XFF, 0XF0, 0XFF, 0XE3, 0X1F, 0XFF, 0XFF, 0XF0, 0XFF, 0XF8, 0XFF, 0XE3, 0X1F,
                    0XFF, 0XFF, 0XF0, 0XFF, 0XF8, 0X7F, 0XE3, 0X1F, 0XFF, 0XFF, 0XF0, 0XFF, 0XF8, 0X7F, 0XC2, 0X1F,
                    0XFF, 0XFF, 0XF8, 0X7F, 0XF8, 0X7F, 0XC0, 0X1F, 0XFF, 0XFF, 0XF8, 0X7F, 0XF8, 0X7F, 0XC4, 0X1F,
                    0XFF, 0XFF, 0XF8, 0X7F, 0XF8, 0X7F, 0XC4, 0X3F, 0XFF, 0XFF, 0XFC, 0X7F, 0XF8, 0X7F, 0XC0, 0X7F,
                    0XFF, 0XFF, 0XFC, 0X7F, 0XF8, 0X7F, 0XC0, 0X7F, 0XFF, 0XFF, 0XFC, 0X7F, 0XF8, 0X7F, 0XC0, 0XFF,
                    0XFF, 0XFF, 0XFC, 0X7F, 0XF8, 0X7F, 0XE0, 0XFF, 0XFF, 0XFF, 0XFC, 0X7F, 0XF8, 0X7F, 0XE0, 0XFF,
                    0XFF, 0XFF, 0XFC, 0X3F, 0XF8, 0X7F, 0XE1, 0XFF, 0XFF, 0XFF, 0XFC, 0X3F, 0XF8, 0X7F, 0XE1, 0XFF,
                    0XFF, 0XFF, 0XFC, 0X3F, 0XF8, 0X7F, 0XC3, 0XFF, 0XFF, 0XFF, 0XFE, 0X1F, 0XF8, 0X7F, 0XC3, 0XFF,
                    0XFF, 0XFF, 0XFE, 0X1F, 0XF0, 0X7F, 0X83, 0XFF, 0XFF, 0XFF, 0XFE, 0X0F, 0XF0, 0X7F, 0X87, 0XFF,
                    0XFF, 0XFF, 0XFF, 0X0F, 0XF0, 0X7F, 0X8F, 0XFF, 0XFF, 0XFF, 0XFF, 0X87, 0XF0, 0X7F, 0X0F, 0XFF,
                    0XFF, 0XFF, 0XFF, 0X87, 0XF0, 0X7E, 0X0F, 0XFF, 0XFF, 0XFF, 0XFF, 0XC3, 0XF0, 0X7E, 0X0F, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XC1, 0XF0, 0X7E, 0X1F, 0XFF, 0XFF, 0XFF, 0XFF, 0XE1, 0XF0, 0X7C, 0X3F, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XE0, 0XF0, 0X7C, 0X3F, 0XFF, 0XFF, 0XFF, 0XFF, 0XF0, 0XF0, 0X78, 0X3F, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XF8, 0X70, 0X70, 0X7F, 0XFF, 0XFF, 0XFF, 0XFF, 0XF8, 0X38, 0X60, 0X7F, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFC, 0X3C, 0X20, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFC, 0X1C, 0X01, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFE, 0X18, 0X03, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFE, 0X18, 0X03, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0X18, 0X07, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0X08, 0X07, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0X08, 0X0F, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0X00, 0X1F, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0X80, 0X3F, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0X80, 0X7F, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XF8, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                    0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF,
                }

            }, cancellationToken: CancellationToken.None);
        }
    }
}
