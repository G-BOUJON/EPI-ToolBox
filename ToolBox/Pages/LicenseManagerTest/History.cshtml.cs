using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToolBox.Models;
using ToolBox.Services.LicenseManagerTest;

namespace ToolBox.Pages.LicenseManager
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        public HistoryModel(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }
        public void OnGet()
        {
        }
        public History getHistory()
        {
            JsonHistoryService jhs = new JsonHistoryService();

            return jhs.getHistory();
        }
    }
}
