using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    public class RestorationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(ServerType id, string? filterJson)
        {

        }
        
    }
}
