using Microsoft.AspNetCore.Mvc;
using ToolBox.Models;
using ToolBox.Services;

namespace ToolBox.Controllers
{
    public class LicenseManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home(ServerType id) 
        {
            return View();
        }

        public IActionResult Configuration(ServerType id)
        {
            return View();
        }

        public IActionResult History(ServerType id) 
        {
            return View();
        }

        public IActionResult UsersList(ServerType id) 
        {
            JsonLoginAccountsService la = new JsonLoginAccountsService(id);
            JsonConfService confService = new JsonConfService(id);
            List<Account> nonExistingAccounts = la.getAccounts().ToList();
            Config config = confService.getConf();
            AccountFilter filter = new AccountFilter();
            
            return View(filter.filterAccounts(nonExistingAccounts, config.maintainedAccounts));
        }
    }
}
