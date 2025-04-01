using MFilesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolBox.Models;
using ToolBox.Services;
using ToolBox_MVC.Areas.LicenseManager.Models;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class SuppressionController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
            return View(new SupressionListModel(id, filter));
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
            SupressionListModel list = new SupressionListModel(id);
            list.UpdateList();
            return RedirectToAction("List",id);
        }

        [HttpPost]
        public IActionResult Maintain(ServerType id, string username)
        {
            JsonConfService confService = new JsonConfService(id);

            if (confService.GetMaintainedAccounts().Contains(username))
            {
                confService.deleteMaintainedAccount(username);
            }
            else
            {
                confService.addMaintainedAccount(username);
            }

            return RedirectToAction("List", id);
        }

        [HttpPost]
        public IActionResult MaintainAllSelected(ServerType id)
        {
            JsonConfService confService = new JsonConfService(id);

            foreach (Account account in new JsonLoginAccountsService(id).GetAccounts().ToList())
            {
                if (!string.IsNullOrEmpty(Request.Form[account.UserName]) && !confService.GetMaintainedAccounts().Contains(account.UserName))
                {
                    confService.addMaintainedAccount(account.UserName);
                }
            }
            return RedirectToAction("List", id);

        }


       
    }
}
