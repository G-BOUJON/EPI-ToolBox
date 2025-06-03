using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory.Sync;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IServerRepository _filesServerRepository;
        private readonly IMfSyncService _syncService;
        private readonly IActiveDirectorySyncService _adSyncService;

        public HomeController(IServerRepository filesServerRepository, IMfSyncService syncService, IActiveDirectorySyncService activeDirectorySyncService)
        {
            _filesServerRepository = filesServerRepository;
            _syncService = syncService;
            _adSyncService = activeDirectorySyncService;
        }
        

        public async Task<IActionResult> Index(string? serverName)
        {
            if (serverName == null)
            {
                serverName = (await _filesServerRepository.GetAllAsync()).First().Name;
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

            await _adSyncService.SyncActiveDirectory(server.ActiveDirectoryID);
            

            await _syncService.SyncAccountsAsync(id);
            await _syncService.SyncGroupsAsync(id);

            return RedirectToAction("Details", new { serverName });
        }
    }
}
