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
        
        public override List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            try
            {
                accounts = Filter.filterAccounts(_accountsListHandler.GetDeletedAccounts().ToList(), Configuration.MaintainedAccounts);

            }
            catch (Exception ex) 
            {
                UpdateAccounts();

                accounts = Filter.filterAccounts(_accountsListHandler.GetDeletedAccounts().ToList(), Configuration.MaintainedAccounts);
            }
            return accounts.OrderBy(o => o.UserName).ToList();
        }

        public override void UpdateList()
        {
            _accountsListHandler.UpdateDeleteList(_usersHandler.GetSuppressionList());
        }
    }
}
