using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class RestorationListModel : AccountList
    {

        public RestorationListModel(ServerType server, IMFilesUsersHandlerFactory mfilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory) : base(server, mfilesFactory, accountsListHandlerFactory) { }

        public RestorationListModel(ServerType server, AccountFilter filter, IMFilesUsersHandlerFactory mfilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory) : base(server, filter, mfilesFactory, accountsListHandlerFactory) { }

        public override List<Account> GetAccounts()
        {
            return (_accountsListHandler.GetRestoredAccounts()).OrderBy(o => o.UserName).ToList();
        }

        public override void UpdateList()
        {
            _accountsListHandler.UpdateRestoreList(_usersHandler.GetRestorationList());
        }
    }
}
