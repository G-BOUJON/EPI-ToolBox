using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class ConfigurationController : Controller
    {
        private readonly IMfilesServerRepository _serverRepo;

        public ConfigurationController(IMfilesServerRepository mfilesServerRepository)
        {
            _serverRepo = mfilesServerRepository;
        }


        public IActionResult Index(string serverName)
        {
            ViewBag.ServerName = serverName;
            return View(_serverRepo.GetServerInfos(serverName));
        }

        public IActionResult ChangeHour(MFilesServer server)
        {
            var dbServer = _serverRepo.GetServerInfos(server.Id);
            dbServer.SyncTime = server.SyncTime;
            _serverRepo.UpdateServer(dbServer);
            return RedirectToAction("Index", new {server.Name});
        }
    }
}
