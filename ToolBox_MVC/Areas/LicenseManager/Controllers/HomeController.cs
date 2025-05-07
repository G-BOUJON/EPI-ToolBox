using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Services.DB;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMfilesServerRepository _filesServerRepository;

        public HomeController(IMfilesServerRepository filesServerRepository)
        {
            _filesServerRepository = filesServerRepository;
        }
        

        public IActionResult Index(string? serverName)
        {
            if (serverName == null)
            {
                ViewData["Test"] = "Master";
                return View();
            }
            return RedirectToAction("Details", new { serverName });
        }

        public IActionResult Details(string serverName)
        {
            return View(_filesServerRepository.GetServerInfos(serverName));
        }
    }
}
