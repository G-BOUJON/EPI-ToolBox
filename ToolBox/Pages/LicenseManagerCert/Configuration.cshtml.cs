using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using ToolBox.Models;
using ToolBox.Services.LicenseManagerCert;

namespace ToolBox.Pages.LicenseManagerCert
{
    [Authorize]
    public class ConfigurationModel : PageModel
    {
        private readonly ILogger<ConfigurationModel> _logger;
        public Config conf { get; set; }
        public string? name { get; set; }
        public string? deleteName { get; set; }

        public ConfigurationModel(ILogger<ConfigurationModel> logger)
        {
            _logger = logger;
            JsonConfService cf = new JsonConfService();
            this.conf = cf.getConf();
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
        }

        public void OnPostDeleteGroup()
        {
            this.deleteGroup(Request.Form["delete"]);
            this.updateData();
        }

        public void OnPostChangeApiConf()
        {
            JsonConfService jcs = new JsonConfService();

            this.conf.apiKey = Request.Form["apiKey"];

            jcs.serializeConfig(this.conf);
        }

        public void OnPostChangeActiveDirectoryCredentials()
        {
            JsonConfService jcs = new JsonConfService();
            this.conf.activeDirectoryCredentials = new ActiveDirectoryCredentials();

            this.conf.activeDirectoryCredentials.username = Request.Form["username"];
            this.conf.activeDirectoryCredentials.password = Request.Form["password"];
            this.conf.activeDirectoryCredentials.domain = Request.Form["domain"];
            this.conf.activeDirectoryCredentials.container = Request.Form["container"];

            jcs.serializeConfig(this.conf);
            this.updateData();
        }

        public void OnPostChangeVaultCredentials()
        {
            JsonConfService jcs = new JsonConfService();
            this.conf.vaultCredentials = new VaultCredentials();
                
            this.conf.vaultCredentials.username = Request.Form["username"];
            this.conf.vaultCredentials.password = Request.Form["password"];
            this.conf.vaultCredentials.domain = Request.Form["domain"];
            this.conf.vaultCredentials.protocolSequence = Request.Form["protocolSequence"];
            this.conf.vaultCredentials.networkAddress = Request.Form["networkAddress"];
            this.conf.vaultCredentials.endPoint = Request.Form["endPoint"];
            this.conf.vaultCredentials.guid = Request.Form["guid"];

            jcs.serializeConfig(this.conf);
            this.updateData();
        }

        public void OnPostSaveParameters()
        {
            JsonConfService cf = new JsonConfService();

            if (Request.Form["active"] == "active")
            {
                cf.changeActivity(true);
            }
            else
            {
                cf.changeActivity(false);
            }
            if (Request.Form["hour"].Count != 0)
            {
                cf.changeHour(Request.Form["hour"]);
            }

            this.conf = cf.getConf();
            this.name = Request.Form["nom"];
        }

        public bool addGroup(string groupName)
        {
            // Initialisation
            bool validGroup = false;
            Group group = new Group(groupName);
            MFilesUsersService mfu = new MFilesUsersService();
            JsonConfService jc = new JsonConfService();

            // Traitement
            if (mfu.groupExists(groupName))
            {
                if (jc.addGroup(group))
                {
                    validGroup = true;
                }
            }

            // Sortie
            return validGroup;
        }

        public void deleteGroup(string groupName)
        {
            JsonConfService jc = new JsonConfService();

            jc.deleteGroup(groupName);
        }

        public void updateData()
        {
            JsonConfService cf = new JsonConfService();
            MFilesUsersService mf = new MFilesUsersService();
            JsonLoginAccountsService la = new();
            la.updateList(mf.getList(cf.getConf().groups));
        }
    }
}
