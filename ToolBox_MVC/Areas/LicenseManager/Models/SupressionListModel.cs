using MFilesAPI;
using ToolBox.Models;
using ToolBox.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class SupressionListModel
    {
        public Config Configuration { get; set; }
        public AccountFilter Filter { get; set; }
        public ServerType Server { get; set; }

        public SupressionListModel(ServerType server) : this(server, new AccountFilter()) { }

        public SupressionListModel(ServerType server, AccountFilter filter)
        {
            Server = server;
            this.Filter = filter;
            Configuration = new JsonConfService(server).GetConf();

        }

        public List<Account> GetNonExistingAccounts()
        {
            return Filter.filterAccounts(new JsonLoginAccountsService(Server).GetAccounts().ToList() , Configuration.maintainedAccounts);
        }

        public void UpdateList()
        {
            MFilesUsersService mf = new MFilesUsersService(Configuration);
            new JsonLoginAccountsService(Server).UpdateList(mf.GetSuppressionList());
        }

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
