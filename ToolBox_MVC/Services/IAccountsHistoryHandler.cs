using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public interface IAccountsHistoryHandler
    {
        TaskHistory GetHistory();
        void AddSuppressedAccount(string accountName);
        void AddRestoredAccount(string accountName);

    }
}
