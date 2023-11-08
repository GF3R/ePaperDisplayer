using EPaper.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.Client.Publishing;
using System.Threading.Tasks;
using EPaper.Web.Core.Services;

namespace EPaper.Web.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController : Controller
    {
        private readonly IMqttService _mqttService;
        
        public ImageController(IMqttService mqttService)
        {
            _mqttService = mqttService;
        }

        [HttpPost]
        public async Task<IActionResult> FromUrlAsBytes(string imageUrl, int width, int height, int threshold = 128)
        {
            var epaperImage = new EPaperImage(imageUrl, width, height, threshold);
            await this._mqttService.Publish(epaperImage.GetEPaperBytes());
            return Ok(epaperImage);
        }

        [HttpPost]
        public async Task<MqttClientPublishResult> Clear()
        {
            return await this._mqttService.Publish(new byte[0]);
        }
        
    }
}
