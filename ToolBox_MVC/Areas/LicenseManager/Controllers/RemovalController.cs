using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    public class RemovalController : Controller
    {
        private readonly ILicenseMangagerService _licenseManager;
        private readonly ISyncService _syncService;

        public RemovalController(ILicenseMangagerService licenseManager, ISyncService syncService)
        {
            _licenseManager = licenseManager;
            _syncService = syncService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(int serverId)
        {
            var accountsToRemove = await _licenseManager.GetAccountsToRemoveLicenseAsync(serverId);
            return View(accountsToRemove.OrderBy(a => a.UserName));
        }
    }
}
