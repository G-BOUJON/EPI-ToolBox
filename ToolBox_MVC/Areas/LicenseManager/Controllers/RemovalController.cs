using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    public class RemovalController : Controller
    {
        private readonly ILicenseMangagerService _licenseManager;
        private readonly ISyncService _syncService;
        private readonly IMfilesServerRepository _serverRepo;

        public RemovalController(ILicenseMangagerService licenseManager, ISyncService syncService, IMfilesServerRepository serverRepo)
        {
            _licenseManager = licenseManager;
            _syncService = syncService;
            _serverRepo = serverRepo;
        }

        /// <summary>
        /// Renders the page with the list of "phatom" accounts on the M-Files server with the name <paramref name="serverName"/>
        /// </summary>
        /// <param name="serverName">The name of the M-Files server</param>
        /// <returns>The <c>Index</c> view with the list of phatom accounts as a model</returns>
        public async Task<IActionResult> Index(string serverName)
        {
            MFilesServer server = _serverRepo.GetServerInfos(serverName);
            var accountsToRemove = await _licenseManager.GetAccountsToRemoveLicenseAsync(server.Id);
            ViewBag.ServerName = serverName;
            return View(accountsToRemove.OrderBy(a => a.UserName));
        }

        public async Task<IActionResult> List(int serverId)
        {
            ViewBag.ServerId = serverId;
            var accountsToRemove = await _licenseManager.GetAccountsToRemoveLicenseAsync(serverId);
            return View(accountsToRemove.OrderBy(a => a.UserName));
        }

        [HttpPost]
        /// <summary>
        /// Ensures that the account with the corresponding <paramref name="accountId"/> is maintained
        /// </summary>
        /// <param name="serverName">The DB name of the current server (only for navigation purpose)</param>
        /// <param name="accountId">The id of the account to maintain</param>
        /// <returns>Redirection to the <c>Index</c> action</returns>
        public IActionResult Maintain(string serverName,int accountId)
        {
            _licenseManager.MaintainAccount(accountId);
            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        /// <summary>
        /// Ensures that the account with the corresponding <paramref name="accountId"/> is not maintained
        /// </summary>
        /// <param name="serverName">The DB name of the current server (only for navigation purpose)</param>
        /// <param name="accountId">The id of the account to unmaintain</param>
        /// <returns>Redirection to the <c>Index</c> action</returns>
        public IActionResult Unmaintain(string serverName, int accountId)
        {
            _licenseManager.UnmaintainAccount(accountId);
            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> MaintainSelection(string serverName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            foreach(var account in await _licenseManager.GetAccountsToRemoveLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName])){
                    _licenseManager.MaintainAccount(account.Id);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> UnmaintainSelection(string serverName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            foreach (var account in await _licenseManager.GetAccountsToRemoveLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    _licenseManager.UnmaintainAccount(account.Id);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLicense(string serverName, string accountName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            await _licenseManager.RemoveLicenseAsync(server.Id, accountName);
            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSelection(string serverName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            foreach (var account in await _licenseManager.GetAccountsToRemoveLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    await _licenseManager.RemoveLicenseAsync(server.Id,account.AccountName);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }
    }
}

