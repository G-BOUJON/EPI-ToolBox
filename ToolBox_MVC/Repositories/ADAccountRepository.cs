using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Repositories
{
    public interface IADAccountRepository : IGenericRepository<ADAccount>
    {
        Task<List<ADAccount>> GetAllInActiveDirectoryAsync(int activeDirectoryID);
        Task<ADAccount> GetByGUIDAsync(string guid);
        Task<ADAccount> GetByNameAsync(int activeDirectoryID, string name);
    }
    public class ADAccountRepository : GenericRepository<ADAccount>,IADAccountRepository
    {
        public ADAccountRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<List<ADAccount>> GetAllInActiveDirectoryAsync(int activeDirectoryID)
        {
            var allAccounts = await _dbSet.Where(a => a.ActiveDirectory.ID == activeDirectoryID).ToListAsync();
            return allAccounts;
        }

        public async Task<ADAccount> GetByGUIDAsync(string guid)
        {
            var account = await _dbSet.FirstAsync(a => a.GUID == guid);
            return account;
        }

        public async Task<ADAccount> GetByNameAsync(int activeDirectoryID, string name)
        {
            return await _dbSet.FirstAsync(a => a.ActiveDirectory.ID == activeDirectoryID && a.Name == name);
        }
    }
}
