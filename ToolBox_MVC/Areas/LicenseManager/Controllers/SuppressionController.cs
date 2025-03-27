using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox.Services;

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

        public IActionResult Liste(ServerType id)
        {
            return View();
        }

       
    }
}
