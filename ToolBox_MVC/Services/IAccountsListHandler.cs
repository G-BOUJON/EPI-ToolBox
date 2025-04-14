using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public interface IAccountsListHandler
    {
        List<IAccount> GetDeletedAccounts();
        List<IAccount> GetRestoredAccounts();

        void UpdateRestoreList(List<LoginAccount> deleteAccounts);
        void UpdateDeleteList(List<LoginAccount> restoreAccounts);

        void UpdateAllAccounts(List<LoginAccount> allAccounts);

        bool IsAccountMaintained(IAccount account);
    }
}
