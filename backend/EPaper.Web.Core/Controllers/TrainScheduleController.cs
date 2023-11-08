namespace EPaper.Web.Core.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Configurations;
    using Services;

    [ApiController]
    [Route("[controller]/[action]")]
    public class TrainScheduleController : Controller
    {
        private readonly IMqttService _mqttService;
        private readonly ImageConfiguration _imageConfiguration;
        private readonly IScheduleService _scheduleService;
        private readonly IImageService _imageService;

        public TrainScheduleController(IMqttService mqttService, ImageConfiguration imageConfiguration, IScheduleService scheduleService, IImageService imageService)
        {
            _mqttService = mqttService;
            _imageConfiguration = imageConfiguration;
            _scheduleService = scheduleService;
            _imageService = imageService;
        }
                
        [HttpGet]
        public async Task<IActionResult> TrainScheduleBern()
        {
            var connections = await _scheduleService.GetConnections("ostermundigen", "bern");
            var image = this._imageService.CreateImageFromConnections(connections);
            var epaperImage = new EPaperImage(image, _imageConfiguration.Width, _imageConfiguration.Height) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
            await this._mqttService.Publish(epaperImage.GetEPaperBytes());
            
            return Ok(connections);
        }
        
        [HttpGet]
        public async Task<IActionResult> TrainScheduleThun()
        {
            var connections = await _scheduleService.GetConnections("ostermundigen", "thun");
            var image = this._imageService.CreateImageFromConnections(connections);
            var epaperImage = new EPaperImage(image, _imageConfiguration.Width, _imageConfiguration.Height) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
            await this._mqttService.Publish(epaperImage.GetEPaperBytes());

            return Ok(connections);
        }
    }
}
