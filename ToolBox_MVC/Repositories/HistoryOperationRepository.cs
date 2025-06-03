using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Repositories
{
    public interface IHistoryOperationRepository : IGenericRepository<HistoryOperation>
    {
        Task<List<HistoryOperation>> GetAllFromAccountAsync(int accountId);
        Task<List<HistoryOperation>> GetAllFromDateAsync(int serverId, DateOnly date);
        Task<List<HistoryOperation>> GetAllInServerAsync(int serverId);
    }

    public class HistoryOperationRepository : GenericRepository<HistoryOperation>, IHistoryOperationRepository
    {
        public HistoryOperationRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<List<HistoryOperation>> GetAllInServerAsync(int serverId)
        {
            var operations = await _dbSet.Include(h => h.Account).Where(o => o.Account.ServerId == serverId).ToListAsync();
            return operations;
        }

        public async Task<List<HistoryOperation>> GetAllFromDateAsync(int serverId, DateOnly date)
        {
            var allOpInServer = await GetAllInServerAsync(serverId);
            return allOpInServer.Where(o => o.Date == date).ToList();
        }

        public async Task<List<HistoryOperation>> GetAllFromAccountAsync(int accountId)
        {
            return await _dbSet.Include(h => h.Account).Where(o => o.AccountID == accountId).ToListAsync();
        }
    }
}
