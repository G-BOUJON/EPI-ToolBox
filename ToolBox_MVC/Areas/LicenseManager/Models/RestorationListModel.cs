using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class RestorationListModel : AccountList
    {
        private const LicenseManagerOperation OPERATION = LicenseManagerOperation.Restoration;

        public RestorationListModel(ServerType server) : base(server) { }

        public RestorationListModel(ServerType server, AccountFilter filter) : base(server, filter) { }

        public override List<Account> GetAccounts()
        {
            return new JsonLoginAccountsService(Server, OPERATION).GetAccounts().ToList();
        }

        public override void UpdateList()
        {
            MFilesUsersService mf = new MFilesUsersService(Configuration);
            new JsonLoginAccountsService(Server, OPERATION).UpdateList(mf.GetRestorationList_V2());
        }
    }
}
