using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Repositories
{
    public interface IADGroupRepository : IGenericRepository<ADGroup>
    {
        Task<List<ADGroup>> GetAllInActiveDirectoryAsync(int activeDirectoryID);
        Task<ADGroup> GetByGUIDAsync(string guid);
        Task<ADGroup> GetByNameAsync(int activeDirectoryID, string name);
    }

    public class ADGroupRepository : GenericRepository<ADGroup>, IADGroupRepository
    {
        public ADGroupRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<List<ADGroup>> GetAllInActiveDirectoryAsync(int activeDirectoryID)
        {
            var allGroup = await _dbSet.Include(g => g.Accounts).Where(a => a.ActiveDirectory.ID == activeDirectoryID).ToListAsync();
            return allGroup;
        }

        public async Task<ADGroup> GetByGUIDAsync(string guid)
        {
            var group = await _dbSet.Include(g => g.Accounts).FirstAsync(a => a.GUID == guid);
            return group;
        }

        public async Task<ADGroup> GetByNameAsync(int activeDirectoryID, string name)
        {
            return await _dbSet.FirstAsync(a => a.ActiveDirectory.ID == activeDirectoryID && a.Name == name);
        }
    }
}
