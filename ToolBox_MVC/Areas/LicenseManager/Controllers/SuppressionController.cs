using MFilesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.JsonServices;

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

        public SuppressionController(IConfigurationHandlerFactory configFactory, IMFilesUsersHandlerFactory mfilesFactory, IAccountsHistoryHandlerFactory accountsHistoryFactory, IAccountsListHandlerFactory accountsListFactory)
        {
            _configurationFactory = configFactory;
            _mfilesFactory = mfilesFactory;
            _accountsHistoryFactory = accountsHistoryFactory;
            _accountsListFactory = accountsListFactory;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
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
        public IActionResult RefreshList(ServerType id)
        {
            UpdateList(id);
            return RedirectToAction("List",new {id});
        }

        [HttpPost]
        public IActionResult Maintain(ServerType id, string username)
        {
            IConfigurationHandler confService = _configurationFactory.Create(id);

            if (confService.GetMaintainedAccounts().Contains(username))
            {
                confService.RemoveMaintainedAccount(username);
            }
            else
            {
                confService.AddMaintainedAccount(username);
            }

            return RedirectToAction("List", new { id });
        }

        [HttpPost]
        public IActionResult MaintainAllSelected(ServerType id)
        {
            IConfigurationHandler confService = _configurationFactory.Create(id);

            foreach (Account account in new JsonLoginAccountsService(id).GetDeletedAccounts())
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]) && !confService.GetMaintainedAccounts().Contains(account.UserName))
                {
                    confService.AddMaintainedAccount(account.UserName);
                }
            }
            return RedirectToAction("List",new { id });
        }

        [HttpPost]
        public IActionResult DeleteLicense(ServerType id, string accountName)
        {
            IMFilesUsersHandler mfUserSevice = _mfilesFactory.Create(id);
            mfUserSevice.DeleteAccountLicense(accountName);
            _accountsHistoryFactory.Create(id).AddSuppressedAccount(accountName);
            UpdateList(id);
            return RedirectToAction("List", new { id });
        }

        [HttpPost]
        public IActionResult DeleteAllSelected(ServerType id)
        {
            IMFilesUsersHandler mfUserService = _mfilesFactory.Create(id);
            JsonLoginAccountsService accountService = new JsonLoginAccountsService(id);

            foreach (Account account in accountService.GetDeletedAccounts())
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    mfUserService.DeleteAccountLicense(account.AccountName);
                    _accountsHistoryFactory.Create(id).AddSuppressedAccount(account.AccountName);
                }
            }
            UpdateList(id);
            return RedirectToAction("List", new { id });
        }

        public IActionResult History(ServerType id)
        {
            ViewData["Server"] = id;
            return View(_accountsHistoryFactory.Create(id).GetHistory());
        }

        private void UpdateList(ServerType id)
        {
            new SuppressionListModel(id, _mfilesFactory, _accountsListFactory).UpdateList();
            GC.Collect();
        }
       
    }
}
