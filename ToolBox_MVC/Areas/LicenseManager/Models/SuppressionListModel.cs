using MFilesAPI;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class SuppressionListModel : AccountList
    {
        
        
        public SuppressionListModel(ServerType server, IMFilesUsersHandlerFactory mFilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory) : base(server, mFilesFactory, accountsListHandlerFactory) { }

        public SuppressionListModel(ServerType server, AccountFilter filter, IMFilesUsersHandlerFactory mFilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory) : base(server, filter, mFilesFactory, accountsListHandlerFactory) { }
        
        public override List<IAccount> GetAccounts()
        {
            List<IAccount> accounts = new List<IAccount>();
            try
            {
                accounts = _accountsListHandler.GetDeletedAccounts().ToList();

            }
            catch (Exception ex) 
            {
                UpdateAccounts();

                accounts = _accountsListHandler.GetDeletedAccounts().ToList();
            }
            return accounts.OrderBy(o => o.UserName).ToList();
        }

        public override void UpdateList()
        {
            _accountsListHandler.UpdateDeleteList(_usersHandler.GetSuppressionList());
        }
    }
}
