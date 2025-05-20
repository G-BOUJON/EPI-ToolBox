using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Areas.LicenseManager.Controllers
{
    [Area("LicenseManager")]
    [Authorize]
    public class ConfigurationController : Controller
    {
        private readonly IServerRepository _serverRepo;
        private readonly IMfCredentialStore _credentialRepo;
        private readonly IADCredentialService _adCredrepo;
        private readonly IGroupRepository _groupRepo;

        public ConfigurationController(IServerRepository mfilesServerRepository, IMfCredentialStore credentialRepository,IADCredentialService aDCredRepository, IGroupRepository groupRepository)
        {
            _serverRepo = mfilesServerRepository;
            _credentialRepo = credentialRepository;
            _adCredrepo = aDCredRepository;
            _groupRepo = groupRepository;
        }


        public async Task<IActionResult> Index(string serverName)
        {
            ViewBag.ServerName = serverName;
            return View(await _serverRepo.GetByNameAsync(serverName));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAutoParameters(MFilesServer server, AutomaticOperations autoParams)
        {
            var dbServer = await _serverRepo.GetByIDAsync(server.Id);
            dbServer.SyncTime = server.SyncTime;
            dbServer.AutomaticOP = autoParams;
            await _serverRepo.SaveChangesAsync();
            return RedirectToAction("Index", new { serverName = dbServer.Name});
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMfCredentials(int serverId, MFilesCredential mfCredentials) 
        {
            var server = await _serverRepo.GetByIDAsync(serverId);

            if (server == null)
            {
                // Redirect somewhere
            }

            if (!string.IsNullOrEmpty(mfCredentials.EncryptedUserName) && !string.IsNullOrEmpty(mfCredentials.EncryptedPassword))
            {
                await _credentialRepo.UpdateCredentials(serverId, mfCredentials);
            }

            return RedirectToAction("Index", new { serverName = server.Name });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMfServerInfos(MFilesServer server)
        {
            var dbServer = await _serverRepo.GetByIDAsync(server.Id);

            dbServer.Name = server.Name;
            dbServer.NetworkAddress = server.NetworkAddress;
            dbServer.ProtocolSequence = server.ProtocolSequence;
            dbServer.EndPoint = server.EndPoint;
            dbServer.VaultGuid = server.VaultGuid;

            await _serverRepo.SaveChangesAsync();
            return RedirectToAction("Index", new { serverName = server.Name });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeADInfos(string serverName, ADCredential adCred)
        {
            var server = await _serverRepo.GetByNameAsync(serverName);

            if (server == null)
            {
                // Redirect somewhere
            }

            server.ADCredential.Container = adCred.Container;
            server.ADCredential.Domain = adCred.Domain;

            await _serverRepo.SaveChangesAsync();

            return RedirectToAction("Index", new { serverName = serverName });
        }


        [HttpPost]
        public async Task<IActionResult> ChangeAdCredentials(string serverName, ADCredential adCred)
        {
            var server = await _serverRepo.GetByNameAsync(serverName);

            if (server == null)
            {
                return RedirectToRoute("LicenseManage/");
            }

            await _adCredrepo.UpdateCredentials(server.Id,adCred);

            return RedirectToAction("Index", new { serverName = serverName });
        }

        [HttpPost]
        public async Task<IActionResult> MaintainGroup(int serverId, int groupId)
        {
            var server = await _serverRepo.GetByIDAsync(serverId);
            var group = await _groupRepo.GetByIDAsync(groupId);

            if (group == null)
            {
                // TODO inclure message erreur ou gestion ?
                return RedirectToAction("Index", new { serverName = server.Name });
            }

            group.Maintained = true;
            await _groupRepo.SaveChangesAsync();

            return RedirectToAction("Index", new { serverName = server.Name });

        }
        [HttpPost]
        public async Task<IActionResult> UnmaintainGroup(int serverId, int groupId)
        {
            var server = await _serverRepo.GetByIDAsync(serverId);
            var group = await _groupRepo.GetByIDAsync(groupId);

            if (group == null)
            {
                // TODO inclure message erreur ou gestion ?
                return RedirectToAction("Index", new { serverName = server.Name });
            }

            group.Maintained = false;
            await _groupRepo.SaveChangesAsync();

            return RedirectToAction("Index", new { serverName = server.Name });

        }

        [HttpPost]
        public async Task<IActionResult> MaintainSelection(int serverId)
        {
            var server = await _serverRepo.GetByIDAsync(serverId);

            foreach(var group in await _groupRepo.GetAllInServerAsync(serverId))
            {
                if (!string.IsNullOrEmpty(Request.Form[group.Name]))
                {
                    group.Maintained = true;
                }
            }

            await _groupRepo.SaveChangesAsync();

            return RedirectToAction("Index", new { serverName = server.Name });
        }

        [HttpPost]
        public async Task<IActionResult> UnmaintainSelection(int serverId)
        {
            var server = await _serverRepo.GetByIDAsync(serverId);

            foreach (var group in await _groupRepo.GetAllInServerAsync(serverId))
            {
                if (!string.IsNullOrEmpty(Request.Form[group.Name]))
                {
                    group.Maintained = false;
                }
            }

            await _groupRepo.SaveChangesAsync();

            return RedirectToAction("Index", new { serverName = server.Name });
        }
    }
}
