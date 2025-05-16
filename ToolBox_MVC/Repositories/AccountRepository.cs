using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Repositories
{
    public interface IAccountRepository : IGenericRepository<MFilesAccount>
    {
        Task<List<MFilesAccount>> GetAllInServerAsync(int serverID);
        Task<MFilesAccount?> GetByAccountNameAsync(int serverID, string accountName);
    }

    public class AccountRepository : GenericRepository<MFilesAccount>, IAccountRepository
    {
        public AccountRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<MFilesAccount?> GetByAccountNameAsync(int serverID, string accountName)
        {
            return await _dbSet.FirstAsync(a => a.AccountName == accountName && a.ServerId == serverID);
        }

        public async Task<List<MFilesAccount>> GetAllInServerAsync(int serverID)
        {
            return await _dbSet.Where(a => a.ServerId == serverID).ToListAsync();
        }
    }
}
