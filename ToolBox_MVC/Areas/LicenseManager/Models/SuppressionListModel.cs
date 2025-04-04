using MFilesAPI;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class SuppressionListModel : AccountList
    {
        public Config Configuration { get; set; }
        public AccountFilter Filter { get; set; }
        public ServerType Server { get; set; }

        public SuppressionListModel(ServerType server) : base(server) { }

        public SuppressionListModel(ServerType server, AccountFilter filter) : base(server, filter) { }
        
        public override List<Account> GetAccounts()
        {
            return Filter.filterAccounts(new JsonLoginAccountsService(Server).GetAccounts().ToList() , Configuration.MaintainedAccounts);
        }

        public override void UpdateList()
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
