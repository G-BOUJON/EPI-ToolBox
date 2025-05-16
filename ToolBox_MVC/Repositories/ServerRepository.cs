using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Repositories
{
    public interface IServerRepository : IGenericRepository<MFilesServer>
    {
        Task<MFilesServer?> GetByNameAsync(string serverName);
    }
    public class ServerRepository : GenericRepository<MFilesServer>, IServerRepository
    {
        public ServerRepository(ToolBoxDbContext context) : base(context)
        {
        }

        public async Task<MFilesServer?> GetByNameAsync(string serverName)
        {
            return await _dbSet.FirstAsync(s => s.Name == serverName);
        }
    }
}
