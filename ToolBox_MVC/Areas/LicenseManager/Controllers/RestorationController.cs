using MFilesAPI;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.JsonServices;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    public class RestorationController : Controller
    {
        private readonly IConfigurationHandlerFactory _configFactory;
        private readonly IMFilesUsersHandlerFactory _mfilesFactory;
        private readonly IAccountsHistoryHandlerFactory _accountsHistoryFactory;
        private readonly IAccountsListHandlerFactory _accountsListFactory;

        public RestorationController(IConfigurationHandlerFactory configFactory, IMFilesUsersHandlerFactory mfilesFactory, IAccountsHistoryHandlerFactory accountsHistoryFactory, IAccountsListHandlerFactory accountsListFactory)
        {
            _configFactory = configFactory;
            _mfilesFactory = mfilesFactory;
            _accountsHistoryFactory = accountsHistoryFactory;
            _accountsListFactory = accountsListFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(ServerType id, string? filterJson)
        {
            AccountFilter filter;
            if (filterJson != null)
            {
                filter = JsonSerializer.Deserialize<AccountFilter>(filterJson);
            }
            else
            {
                filter = new AccountFilter();
            }
            return View(new RestorationListModel(id, filter, _mfilesFactory, _accountsListFactory));
        }

        public IActionResult History(ServerType id)
        {
            ViewData["Server"] = id;
            return View(_accountsHistoryFactory.Create(id).GetHistory());
        }

        [HttpPost]
        public IActionResult RefreshList(ServerType id)
        {
            UpdateList(id);
            return RedirectToAction("List", new { id });
        }

        [HttpPost]
        public IActionResult RestoreLicense(ServerType id, string accountName)
        {
            IMFilesUsersHandler mfServices = _mfilesFactory.Create(id);
            IAccountsHistoryHandler historyService = _accountsHistoryFactory.Create(id);

            mfServices.RestoreAccountLicense(accountName);
            historyService.AddRestoredAccount(accountName);

            UpdateList(id);

            return RedirectToAction("List", new { id });
            
        }

        [HttpPost]
        // TODO : Tester la méthode, voir pour supprimer 3-4 licences et le remettres depuis le site
        public IActionResult RestoreAllLicense(ServerType id)
        {
            IMFilesUsersHandler mfUserService = _mfilesFactory.Create(id);
            JsonLoginAccountsService accountService = new JsonLoginAccountsService(id);
            IAccountsHistoryHandler historyService = _accountsHistoryFactory.Create(id);

            foreach (Account account in accountService.GetRestoredAccounts())
            {
                if (!string.IsNullOrEmpty(Request.Form[account.AccountName]))
                {
                    mfUserService.RestoreAccountLicense(account.AccountName);
                    historyService.AddRestoredAccount(account.AccountName);
                }
            }
            UpdateList(id);
            return RedirectToAction("List", new { id });
        }



        private void UpdateList(ServerType id)
        {
            new RestorationListModel(id, _mfilesFactory, _accountsListFactory).UpdateList();
            GC.Collect();
        }
        
    }
}
