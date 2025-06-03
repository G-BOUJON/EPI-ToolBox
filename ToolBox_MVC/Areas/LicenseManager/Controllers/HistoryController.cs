using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Authorize]
    [Area("LicenseManager")]
    public class HistoryController : Controller
    {
        private readonly IOperationHistoryService _operationHistoryService;
        private readonly IServerRepository _serverRepository;

        public HistoryController(IOperationHistoryService operationHistoryService, IServerRepository serverRepository)
        {
            _operationHistoryService = operationHistoryService;
            _serverRepository = serverRepository;
        }

        public async Task<IActionResult> Index(string serverName)
        {
            var server = await _serverRepository.GetByNameAsync(serverName);

            if (server == null)
            {
                return NotFound();
            }

            ViewBag.ServerName = serverName;
            var dateDict = await _operationHistoryService.GetDateAndCountsAsync(server.Id);

            return View(dateDict);
        }

        [Route("/MFiles/{serverName}/Date/{dateInt}")]
        public async Task<IActionResult> Date(string serverName, int dateInt)
        {
            var server = await _serverRepository.GetByNameAsync(serverName);

            if (server == null)
            {
                return NotFound();
            }
            DateOnly date = DateOnly.FromDayNumber(dateInt);

            ViewBag.ServerName = serverName;
            ViewBag.Date = date;

            var dateOperations = (await _operationHistoryService.GetAllFromDateAsync(server.Id, date)).OrderByDescending(o => o.Time);
            if (dateOperations.Count() <= 0)
            {
                return RedirectToAction("Index", new { serverName });
            }


            return View(dateOperations);
        }

        [Route("{accountId}")]
        public async Task<IActionResult> Account(string serverName, int accountId)
        {
            var server = await _serverRepository.GetByNameAsync(serverName);

            if (server == null)
            {
                return NotFound();
            }

            var accountHistory = (await _operationHistoryService.GetAllFromAccountAsync(accountId)).OrderByDescending(o => o.Date);
            if (accountHistory.Count() <= 0)
            {
                return RedirectToAction("Index", new { serverName });
            }
            ViewBag.ServerName = serverName;
            ViewBag.Account = accountHistory.First().Account;

            return View(accountHistory);
        }
    }
}
