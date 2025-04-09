using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public interface IAccountsListHandler
    {
        List<Account> GetDeletedAccounts();
        List<Account> GetRestoredAccounts();

        void UpdateRestoreList(List<LoginAccount> deleteAccounts);
        void UpdateDeleteList(List<LoginAccount> restoreAccounts);

        void UpdateAccounts(Accounts accounts);
    }
}
