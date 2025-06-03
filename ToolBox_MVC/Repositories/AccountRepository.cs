using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Repositories
{
    public interface IAccountRepository : IGenericRepository<MFilesAccount>
    {
        Task<List<MFilesAccount>> GetAllInServerAsync(int serverID);
        Task<List<MFilesAccount>> GetAllInServerIncludeAsync(int serverID);
        Task<MFilesAccount> GetByAccountNameAsync(int serverID, string accountName);
        Task<MFilesAccount> GetByMfIDAsync(int serverID, int mfID);
    }

    public class AccountRepository : GenericRepository<MFilesAccount>, IAccountRepository
    {
        public AccountRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<MFilesAccount> GetByAccountNameAsync(int serverID, string accountName)
        {
            return await _dbSet.Include(a => a.ADAccount).FirstAsync(a => a.AccountName == accountName && a.ServerId == serverID);
        }

        public async Task<List<MFilesAccount>> GetAllInServerAsync(int serverID)
        {
            return await _dbSet.Where(a => a.ServerId == serverID).ToListAsync();
        }

        public async Task<List<MFilesAccount>> GetAllInServerIncludeAsync(int serverID)
        {
            return await _dbSet.Include(a => a.ADAccount).Where(a => a.ServerId == serverID).ToListAsync();
        }

        public async Task<MFilesAccount> GetByMfIDAsync(int serverID, int mfID)
        {
            return await _dbSet.Include(a => a.ADAccount).FirstAsync(a => a.UserId == mfID && a.ServerId == serverID);
        }

        public override async Task<MFilesAccount?> GetByIDAsync(int id)
        {
            MFilesAccount? account;
            try
            {

                account = await _dbSet.Include(a => a.ADAccount).FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (ArgumentNullException)
            {
                account = null;
            }
            return account;
        }
    }
}
