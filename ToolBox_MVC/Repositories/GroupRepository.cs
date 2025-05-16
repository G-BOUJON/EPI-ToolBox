using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Repositories
{
    public interface IGroupRepository : IGenericRepository<MFilesGroup>
    {
        Task<MFilesGroup?> GetByMfIDAsync(int serverID, int mfID);
        Task<MFilesGroup?> GetByMfIDIncludeAccountsAsync(int serverID, int mfID);
        Task<MFilesGroup?> GetByIdIncludeAccountAsync(int id);
        Task<List<MFilesGroup>> GetAllInServerAsync(int serverID);
        Task<List<MFilesGroup>> GetAllInServerIncludeAccountsAsync(int serverID);
    }
    public class GroupRepository : GenericRepository<MFilesGroup>, IGroupRepository
    {
        public GroupRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<List<MFilesGroup>> GetAllInServerAsync(int serverID)
        {
            return await _dbSet.Where(g => g.ServerId == serverID).ToListAsync();
        }

        public async Task<List<MFilesGroup>> GetAllInServerIncludeAccountsAsync(int serverID)
        {
            return await _dbSet.Include(g => g.Accounts).Where(g => g.ServerId == serverID).ToListAsync();
        }

        public async Task<MFilesGroup?> GetByIdIncludeAccountAsync(int id)
        {
            return await _dbSet.Include(g => g.Accounts).FirstAsync(g => g.Id == id);
        }

        public async Task<MFilesGroup?> GetByMfIDAsync(int serverID, int mfID)
        {
            return await _dbSet.FirstAsync(g => g.ServerId == serverID && g.MFilesId == mfID);
        }

        public async Task<MFilesGroup?> GetByMfIDIncludeAccountsAsync(int serverID, int mfID)
        {
            return await _dbSet.Include(g=>g.Accounts).FirstAsync(g => g.ServerId == serverID && g.MFilesId == mfID);
        }
    }
}
