using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMfilesServerRepository _filesServerRepository;
        private readonly ISyncService _syncService;

        public HomeController(IMfilesServerRepository filesServerRepository, ISyncService syncService)
        {
            _filesServerRepository = filesServerRepository;
            _syncService = syncService;
        }
        

        public IActionResult Index(string? serverName)
        {
            if (serverName == null)
            {
                ViewData["Test"] = "Master";
                return View();
            }
            return RedirectToAction("Details", new { serverName });
        }

        public IActionResult Details(string serverName)
        {
            return View(_filesServerRepository.GetServerInfos(serverName));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(string serverName)
        {
            var server = _filesServerRepository.GetServerInfos(serverName);
            var id = server.Id;

            await _syncService.SyncAccountsAsync(id);
            await _syncService.SyncGroupsAsync(id);
            

            return RedirectToAction("Details", new { serverName });
        }
    }
}
