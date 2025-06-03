using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Areas.LicenseManager.Services
{
    public interface IOperationHistoryService
    {
        Task<List<HistoryOperation>> GetAllFromAccountAsync(int accountId);
        Task<List<HistoryOperation>> GetAllFromDateAsync(int serverId, DateOnly date);
        Task<List<HistoryOperation>> GetAllFromServerAsync(int serverId);
        Task<Dictionary<DateOnly, int>> GetDateAndCountsAsync(int serverId);
        Task RegisterNewOperation(MFilesAccount account, OperationType operationType, bool automatic = false);
        Task SaveChanges();
    }
}