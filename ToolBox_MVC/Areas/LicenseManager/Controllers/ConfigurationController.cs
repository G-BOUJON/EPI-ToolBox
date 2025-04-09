using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class ConfigurationController : Controller
    {
        private readonly IMFilesUsersHandlerFactory _mfilesFactory;
        private readonly IConfigurationHandlerFactory _configFactory;

        public ConfigurationController(IMFilesUsersHandlerFactory mfilesFactory, IConfigurationHandlerFactory configFactory)
        {
            _mfilesFactory = mfilesFactory;
            _configFactory = configFactory;
        }

        [Route("LicenseManager/Configuration/{id}")]
        public IActionResult Index(ServerType id, string? errorMessage)
        {
            if (errorMessage != null)
            {
                ViewData["ErrorMessage"] = errorMessage;
            }
            ViewData["Server"] = id;
            return View(_configFactory.Create(id).GetConfiguration());
        }

        [HttpPost]
        public IActionResult AddGroup(ServerType id)
        {
            string newGroupName = Request.Form["groupName"];
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();
            IMFilesUsersHandler mf = _mfilesFactory.Create(id);


            if (string.IsNullOrEmpty(newGroupName))
            {
                return RedirectToAction("Index", new
                {
                    id,
                    errorMessage = "Nom de groupe vide"
                });
            }

            if (!mf.GroupExists(newGroupName))
            {
                return RedirectToAction("Index", new
                {
                    id,
                    errorMessage = "Groupe inexistant"
                });
            }

            
            Group newGroup = new Group(newGroupName);

            if (configuration.Groups.Contains(newGroup))
            {
                return RedirectToAction("Index", new
                {
                    id,
                    errorMessage = "Groupe déjà ajouté à la configuration"
                });
            }

            configuration.Groups.Add(newGroup);
            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult DeleteGroup(ServerType id, string groupName)
        {
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();
            List<Group> lstGroup = new(configuration.Groups);

            foreach (Group group in lstGroup)
            {
                if (group.name == groupName)
                {
                    configuration.Groups.Remove(group);
                }
            }


            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditApiKey(ServerType id, string apiKey)
        {
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();
            configuration.ApiKey = apiKey;

            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditADCredentials(ServerType id, ActiveDirectoryCredentials credentials)
        {
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();

            configuration.ActiveDirectoryCredentials = credentials;

            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditVaultCredentials(ServerType id,  VaultCredentials credentials)
        {
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();

            configuration.VaultCredentials = credentials;

            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditAutomatisation(ServerType id, Config autoConfig)
        {
            IConfigurationHandler jsConf = _configFactory.Create(id);
            Config configuration = jsConf.GetConfiguration();

            configuration.Hour = autoConfig.Hour;
            configuration.ActiveSuppression = autoConfig.ActiveSuppression;
            configuration.ActiveRestauration = autoConfig.ActiveRestauration;

            jsConf.UpdateConfiguration(configuration);

            return RedirectToAction("Index", new { id });
        }
    }
}
