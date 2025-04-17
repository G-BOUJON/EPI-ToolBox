using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services.DB
{
    public interface IAccountsRepository
    {
        /// <summary>
        /// Synchronize DB with incoming Accounts List
        /// </summary>
        /// <param name="serverId">ID of the server which is targeted by the sync</param>
        /// <param name="currentAccounts">List of incoming accounts</param>
        /// <returns></returns>
        Task SyncAccountsAsync(int serverId, List<MFilesAccount> currentAccounts);

        /// <summary>
        /// Fetch all accounts on the DB with the selected serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        Task<List<MFilesAccount>> GetUserAsync(int serverId);
    }
}
