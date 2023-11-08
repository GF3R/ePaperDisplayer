namespace EPaper.Web.Core.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Configurations;
    using Services;
    using Services.Desk;

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeskController : Controller
    {
        private readonly IMqttService _mqttService;
        private readonly ImageConfiguration _imageConfiguration;
        private readonly IImageService _imageService;
        private readonly IDeskService _deskService;

        public DeskController(IMqttService mqttService, ImageConfiguration imageConfiguration, IImageService imageService, IDeskService deskService)
        {
            _mqttService = mqttService;
            _imageConfiguration = imageConfiguration;
            _imageService = imageService;
            _deskService = deskService;
        }
        
        [HttpPost]
        public async Task<IActionResult> DisplayDeskAvailability(string deskId, string microControllerId)
        {
            var desk = await _deskService.GetDeskModelFromDeskId(deskId);
            var image = this._imageService.CreateImageFromDeskModel(desk);
            var epaperImage = new EPaperImage(image, 200, 200) { WhiteToBlackThrehshold = _imageConfiguration.Threshold };
            await this._mqttService.Publish(epaperImage.GetEPaperBytes());
            return Ok(epaperImage);
        }
    }
}
