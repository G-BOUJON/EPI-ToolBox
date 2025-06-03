using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Areas.LicenseManager.Services
{
    public class OperationHistoryService : IOperationHistoryService
    {
        private readonly IHistoryOperationRepository _opRepo;

        public OperationHistoryService(IHistoryOperationRepository opRepo)
        {
            _opRepo = opRepo;
        }

        public async Task<List<HistoryOperation>> GetAllFromServerAsync(int serverId)
        {
            return await _opRepo.GetAllInServerAsync(serverId);
        }

        public async Task<List<HistoryOperation>> GetAllFromDateAsync(int serverId, DateOnly date)
        {
            return await _opRepo.GetAllFromDateAsync(serverId, date);
        }

        public async Task<List<HistoryOperation>> GetAllFromAccountAsync(int accountId)
        {
            return await _opRepo.GetAllFromAccountAsync(accountId);
        }

        public async Task<Dictionary<DateOnly, int>> GetDateAndCountsAsync(int serverId)
        {
            var allOperations = await GetAllFromServerAsync(serverId);

            var groupedOperations = allOperations.GroupBy(o => o.Date).OrderByDescending(o => o.Key);

            Dictionary<DateOnly, int> operationsDict = groupedOperations.ToDictionary(g => g.Key, g => g.Count());

            return operationsDict;
        }

        public async Task RegisterNewOperation(MFilesAccount account, OperationType operationType, bool automatic = false)
        {
            HistoryOperation operationToRegister = new HistoryOperation(DateTime.Now, (int)operationType, account, automatic);
            await _opRepo.AddAsync(operationToRegister);
            await _opRepo.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _opRepo.SaveChangesAsync();
        }

    }

    public enum OperationType
    {
        Removal = 1,
        Restoration = 2,
        Activation = 3
    }
}
