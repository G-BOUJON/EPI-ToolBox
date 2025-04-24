using MFilesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.JsonServices;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class SuppressionController : Controller
    {
        private readonly IConfigurationHandlerFactory _configurationFactory;
        private readonly IMFilesUsersHandlerFactory _mfilesFactory;
        private readonly IAccountsHistoryHandlerFactory _accountsHistoryFactory;
        private readonly IAccountsListHandlerFactory _accountsListFactory;
        private readonly ISyncService _syncService;
        private readonly ILicenseMangagerService _licenseMangagerService;

        public SuppressionController(IConfigurationHandlerFactory configFactory, IMFilesUsersHandlerFactory mfilesFactory, IAccountsHistoryHandlerFactory accountsHistoryFactory, IAccountsListHandlerFactory accountsListFactory, ISyncService syncService, ILicenseMangagerService licenseMangagerService)
        {
            _configurationFactory = configFactory;
            _mfilesFactory = mfilesFactory;
            _accountsHistoryFactory = accountsHistoryFactory;
            _accountsListFactory = accountsListFactory;
            _syncService = syncService;
            _licenseMangagerService = licenseMangagerService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> Test(int id)
        {
            return View((await _licenseMangagerService.GetAccountsToRemoveLicenseAsync(id)).ToList().OrderBy(a => a.UserName));
        }

        public async Task<IActionResult> TestRemoveLicense(int id, string accountName)
        {
            await _licenseMangagerService.RemoveLicenseAsync((int)id, accountName);
            return RedirectToAction("Test", new {id});
        }
        

        public IActionResult List(ServerType id, string? JSfilter)
        {
            AccountFilter filter;
            if (JSfilter != null)
            {
                filter = JsonSerializer.Deserialize<AccountFilter>(JSfilter);
            }
            else
            {
                filter = new AccountFilter();
            }
            return View(new SuppressionListModel(id, filter, _mfilesFactory, _accountsListFactory));
        }

        [HttpPost]
        public IActionResult ChangeFilter(ServerType id) 
        {
            AccountFilter filter = new AccountFilter();

            List<MFLicenseType> licenseTypes = new List<MFLicenseType>();
            List<MFLoginAccountType> loginTypes = new List<MFLoginAccountType>();
            List<MaintainedAccountType> maintainedTypes = new List<MaintainedAccountType>();

            foreach (MFLicenseType licenseType in filter.LicenseTypes)
            {
                if (Request.Form[licenseType.ToString()] == "on")
                {
                    licenseTypes.Add(licenseType);
                }
            }
            if (licenseTypes.Count > 0)
                filter.LicenseTypes = licenseTypes;
            

            foreach (MFLoginAccountType loginType in filter.AccountTypes)
            {
                if (Request.Form[loginType.ToString()] == "on")
                {
                    loginTypes.Add(loginType);
                }
            }
            if (loginTypes.Count > 0)
                filter.AccountTypes = loginTypes;

            foreach (MaintainedAccountType maintainedType in filter.MaintainedTypes)
            {
                if (Request.Form[maintainedType.ToString()] == "on")
                {
                    maintainedTypes.Add(maintainedType);
                }
            }
            if (maintainedTypes.Count > 0)
                filter.MaintainedTypes = maintainedTypes;

            string JSfilter = JsonSerializer.Serialize(filter);

            return RedirectToAction("List", new
            {
                id = id,
                JSfilter = JSfilter,
            });
        }

        [HttpPost]
        public async Task<IActionResult> RefreshList(int id)
        {
            await _syncService.SyncAccountsAsync(id);
            await _syncService.SyncGroupsAsync(id);
            await _syncService.SyncGroupsAccountsLinksAsync(id);

            return RedirectToAction("Test",new {id});
        }

        public async Task<IActionResult> Sync(int id)
        {
            await _syncService.SyncAccountsAsync(id);
            await _syncService.SyncGroupsAsync(id);
            await _syncService.SyncGroupsAccountsLinksAsync(id);

            return RedirectToAction("Test", new { id });
        }

        [HttpPost]
        public IActionResult Maintain(int id, string accountName)
        {
            _licenseMangagerService.MaintainAccount(id, accountName);

            return RedirectToAction("Test", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> MaintainAllSelected(int id)
        {
            foreach (MFilesAccount account in await _licenseMangagerService.GetAccountsToRemoveLicenseAsync(id))
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    _licenseMangagerService.MaintainAccount(id,account.AccountName);
                }
            }
            return RedirectToAction("Test",new { id });
        }

        [HttpPost]
        public IActionResult DeleteLicense(ServerType id, string accountName)
        {
            IMFilesUsersHandler mfUserSevice = _mfilesFactory.Create(id);
            mfUserSevice.DeleteAccountLicense(accountName);
            _accountsHistoryFactory.Create(id).AddSuppressedAccount(accountName);
            
            return RedirectToAction("List", new { id });
        }

        [HttpPost]
        public IActionResult DeleteAllSelected(ServerType id)
        {
            IMFilesUsersHandler mfUserService = _mfilesFactory.Create(id);
            IAccountsListHandler accountService = _accountsListFactory.Create(id);

            foreach (IAccount account in accountService.GetDeletedAccounts())
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    mfUserService.DeleteAccountLicense(account.AccountName);
                    _accountsHistoryFactory.Create(id).AddSuppressedAccount(account.AccountName);
                }
            }
            
            return RedirectToAction("List", new { id });
        }

        public IActionResult History(ServerType id)
        {
            ViewData["Server"] = id;
            return View(_accountsHistoryFactory.Create(id).GetHistory());
        }

        
       
    }
}
