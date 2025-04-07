using MFilesAPI;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class SuppressionListModel : AccountList
    {
        private const LicenseManagerOperation OPERATION = LicenseManagerOperation.Suppression;
        public SuppressionListModel(ServerType server) : base(server) { }

        public SuppressionListModel(ServerType server, AccountFilter filter) : base(server, filter) { }
        
        public override List<Account> GetAccounts()
        {
            return Filter.filterAccounts(new JsonLoginAccountsService(Server,OPERATION).GetAccounts().ToList() , Configuration.MaintainedAccounts);
        }

        public override void UpdateList()
        {
            MFilesUsersService mf = new MFilesUsersService(Configuration);
            new JsonLoginAccountsService(Server, OPERATION).UpdateList(mf.GetSuppressionList());
        }
    }
}
