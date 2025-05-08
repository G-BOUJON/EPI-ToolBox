using MFilesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.JsonServices;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class RestorationController : Controller
    {
        private readonly ILicenseMangagerService _licenseManager;
        private readonly IMfilesServerRepository _serverRepo;
        

        public RestorationController(ILicenseMangagerService licenseManager, IMfilesServerRepository serverRepo)
        {
            _licenseManager = licenseManager;
            _serverRepo = serverRepo;
            
        }

        public async Task<IActionResult> Index(string serverName)
        {
            MFilesServer server = _serverRepo.GetServerInfos(serverName);
            IEnumerable<MFilesAccount> accountsToRestore = await _licenseManager.GetAccountsToRestoreLicenseAsync(server.Id);
            ViewBag.ServerName = serverName;
            return View(accountsToRestore.OrderBy(a => a.UserName));
        }

        [HttpPost]
        /// <summary>
        /// Ensures that the account with the corresponding <paramref name="accountId"/> is maintained
        /// </summary>
        /// <param name="serverName">The DB name of the current server (only for navigation purpose)</param>
        /// <param name="accountId">The id of the account to maintain</param>
        /// <returns>Redirection to the <c>Index</c> action</returns>
        public IActionResult Maintain(string serverName, int accountId)
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

            foreach (var account in await _licenseManager.GetAccountsToRestoreLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    _licenseManager.MaintainAccount(account.Id);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> UnmaintainSelection(string serverName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            foreach (var account in await _licenseManager.GetAccountsToRestoreLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    _licenseManager.UnmaintainAccount(account.Id);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> RestoreLicense(string serverName, string accountName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            await _licenseManager.RestoreLicenseAsync(server.Id, accountName);
            return RedirectToAction("Index", new { serverName });
        }

        [HttpPost]
        public async Task<IActionResult> RestoreSelection(string serverName)
        {
            var server = _serverRepo.GetServerInfos(serverName);

            foreach (var account in await _licenseManager.GetAccountsToRestoreLicenseAsync(server.Id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    await _licenseManager.RestoreLicenseAsync(server.Id, account.AccountName);
                }
            }

            return RedirectToAction("Index", new { serverName });
        }
    }
}
