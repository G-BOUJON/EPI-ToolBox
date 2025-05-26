using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IServerRepository _filesServerRepository;
        private readonly ISyncService _syncService;

        public HomeController(IServerRepository filesServerRepository, ISyncService syncService)
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

        public async Task<IActionResult> Details(string serverName)
        {
            return View( await _filesServerRepository.GetByNameAsync(serverName));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(string serverName)
        {
            var server = await _filesServerRepository.GetByNameAsync(serverName);
            var id = server.Id;

            await _syncService.SyncAccountsAsync(id);
            await _syncService.SyncGroupsAsync(id);
            

            return RedirectToAction("Details", new { serverName });
        }
    }
}
