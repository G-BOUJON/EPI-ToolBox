using Microsoft.AspNetCore.Mvc;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Test"] = "Master";
            return View();
        }
    }
}
