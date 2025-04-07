﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;
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
            AccountFilter filter;
            if (filterJson != null)
            {
                filter = JsonSerializer.Deserialize<AccountFilter>(filterJson);
            }
            else
            {
                filter = new AccountFilter();
            }
            return View(new RestorationListModel(id, filter));
        }

        [HttpPost]
        public IActionResult RefreshList(ServerType id)
        {
            UpdateList(id);
            return RedirectToAction("List", new { id });
        }



        private void UpdateList(ServerType id)
        {
            new RestorationListModel(id).UpdateList();
            GC.Collect();
        }
        
    }
}
