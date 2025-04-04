using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class ConfigurationController : Controller
    {
        [Route("LicenseManager/Configuration/{id}")]
        public IActionResult Index(ServerType id, string? errorMessage)
        {
            if (errorMessage != null)
            {
                ViewData["ErrorMessage"] = errorMessage;
            }
            ViewData["Server"] = id;
            return View(new JsonConfService(id).GetConf());
        }

        [HttpPost]
        public IActionResult AddGroup(ServerType id)
        {
            string newGroupName = Request.Form["groupName"];
            JsonConfService jsConf = new JsonConfService(id);
            Config configuration = jsConf.GetConf();
            MFilesUsersService mf = new MFilesUsersService(configuration);


            if (string.IsNullOrEmpty(newGroupName))
            {
                return RedirectToAction("Index", new
                {
                    id,
                    errorMessage = "Nom de groupe vide"
                });
            }

            if (!mf.groupExists(newGroupName))
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
            jsConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult DeleteGroup(ServerType id, string groupName)
        {
            JsonConfService jsonConf = new JsonConfService(id);
            Config configuration = jsonConf.GetConf();
            List<Group> lstGroup = new(configuration.Groups);

            foreach (Group group in lstGroup)
            {
                if (group.name == groupName)
                {
                    configuration.Groups.Remove(group);
                }
            }


            jsonConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditApiKey(ServerType id, string apiKey)
        {
            JsonConfService jsConf = new JsonConfService(id);
            Config configuration = jsConf.GetConf();
            configuration.ApiKey = apiKey;

            jsConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditADCredentials(ServerType id, ActiveDirectoryCredentials credentials)
        {
            JsonConfService jsConf = new JsonConfService(id);
            Config configuration = jsConf.GetConf();

            configuration.ActiveDirectoryCredentials = credentials;

            jsConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditVaultCredentials(ServerType id,  VaultCredentials credentials)
        {
            JsonConfService jsonConf = new JsonConfService(id);
            Config configuration = jsonConf.GetConf();

            configuration.VaultCredentials = credentials;

            jsonConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public IActionResult EditAutomatisation(ServerType id, Config autoConfig)
        {
            JsonConfService jsonConf = new JsonConfService(id);
            Config configuration = jsonConf.GetConf();

            configuration.Hour = autoConfig.Hour;
            configuration.ActiveSuppression = autoConfig.ActiveSuppression;
            configuration.ActiveRestauration = autoConfig.ActiveRestauration;

            jsonConf.serializeConfig(configuration);

            return RedirectToAction("Index", new { id });
        }
    }
}
