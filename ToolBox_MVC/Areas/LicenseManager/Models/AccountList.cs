using MFilesAPI;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public abstract class AccountList
    {
        public Config Configuration { get; set; }
        public AccountFilter Filter { get; set; }
        public ServerType Server { get; set; }

        public AccountList(ServerType server) : this(server, new AccountFilter()) { }

        public AccountList(ServerType server, AccountFilter filter)
        {
            Server = server;
            this.Filter = filter;
            Configuration = new JsonConfService(Server).GetConf();

        }

        public abstract List<Account> GetAccounts();
        public abstract void UpdateList();
        

        public bool FilterContains(MFLicenseType licenseType)
        {
            return Filter.LicenseTypes.Contains(licenseType);
        }

        public bool FilterContains(MFLoginAccountType accountType)
        {
            return Filter.AccountTypes.Contains(accountType);
        }

        public bool FilterContains(MaintainedAccountType maintainedAccountType)
        {
            return Filter.MaintainedTypes.Contains(maintainedAccountType);
        }
    }
}
