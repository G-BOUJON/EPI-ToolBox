using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToolBox.Models;
using ToolBox.Services.LicenseManagerTest;

namespace ToolBox.Controllers.LicenseManagerTest
{
    [Route("LicenseManagerTest/api/configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        public JsonConfService ConfService { get; set; }
        public ConfigurationController(JsonConfService confService)
        {
            this.ConfService = confService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string token)
        {
            Config config = this.ConfService.getConf();

            if (config.apiKey == token)
            {
                return Ok(this.ConfService.getConf());
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpDelete("groups")]
        public ActionResult DeleteGroup([FromQuery] string token, [FromQuery] string groupName)
        {
            Config config = this.ConfService.getConf();

            if (config.apiKey == token)
            {
                ConfService.deleteGroup(groupName);
                updateData();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpPost("groups")]
        public ActionResult PostGroup([FromQuery] string token, [FromQuery] string groupName)
        {
            Config config = this.ConfService.getConf();

            if(config.apiKey == token)
            {
                Group newGroup = new(groupName);
                ConfService.addGroup(newGroup);
                updateData();
                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpPost("active")]
        public ActionResult PostActivity([FromQuery] string token, [FromQuery] bool active)
        {
            Config config = this.ConfService.getConf();

            if (config.apiKey == token)
            {
                ConfService.changeActivity(active);
                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpPost("frequence")]
        public ActionResult PostFrequence([FromQuery] string token, [FromQuery] int frequence)
        {
            Config config = this.ConfService.getConf();

            if (config.apiKey == token)
            {
                ConfService.changeFrequence(frequence);
                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpPost("hour")]
        public ActionResult PostHour([FromQuery] string token, [FromQuery] string hour)
        {
            Config config = this.ConfService.getConf();

            if (config.apiKey == token)
            {
                ConfService.changeHour(hour);
                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        public void updateData()
        {
            MFilesUsersService mf = new MFilesUsersService();
            JsonLoginAccountsService la = new();
            la.updateList(mf.getList(this.ConfService.getConf().groups));
        }
    }
}
