using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult List(ServerType id)
        {
            return View(new SupressionListModel(id));
        }

        [HttpPost]
        public IActionResult RefreshList(ServerType id)
        {
            SupressionListModel list = new SupressionListModel(id);
            list.UpdateList();
            return RedirectToAction("List",id);
        }


       
    }
}
