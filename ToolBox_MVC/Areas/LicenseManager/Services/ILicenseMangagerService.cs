using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Areas.LicenseManager.Services
{
    public interface ILicenseMangagerService
    {
        Task<IEnumerable<MFilesAccount>> GetAccountsToRemoveLicenseAsync(int serverId);
        Task<IEnumerable<MFilesAccount>> GetAccountsToRestoreLicenseAsync(int serverId);
        Task RemoveLicenseAsync(int serverId, string accountName);
        Task RestoreLicenseAsync(int serverId, string accountName);
        void MaintainAccount(int serverId, string accountName);
        void UnmaintainAccount(int serverId, string accountName);
    }
}
