using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.MFiles;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class ActivationController : Controller
    {
        private readonly IMfilesAccountActivationHandler _activationHandler;
        private readonly IServerRepository _serverRepo;

        public ActivationController(IMfilesAccountActivationHandler activationHandler, IServerRepository serverRepository)
        {
            _activationHandler = activationHandler;
            _serverRepo = serverRepository;
        }

        public async Task<IActionResult> Index(string serverName)
        {
            var server = await _serverRepo.GetByNameAsync(serverName);

            IEnumerable<MFilesAccount> accountsToDeactivate = await _activationHandler.GetAccountsToReactivateAsync(server.Id);

            ViewBag.ServerName = serverName;
            

            return View(accountsToDeactivate.OrderBy(a => a.UserName));
        }
    }
}
