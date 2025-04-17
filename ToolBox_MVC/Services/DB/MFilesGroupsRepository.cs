using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Services.DB
{
    public class MFilesGroupsRepository : IGroupRepository
    {
        private readonly ToolBoxDbContext _dbContext;

        public MFilesGroupsRepository(ToolBoxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SyncGroupsAsync(int serverId, List<MFilesGroup> incomingGroups)
        {
            var existingGroups = await GetGroupsAsync(serverId);

            var existingDict = existingGroups.ToDictionary(g => g.MFilesId, g=>g);
            var incomingDict = incomingGroups.ToDictionary(g => g.MFilesId, g => g);

            foreach (var group in incomingGroups)
            {
                if (existingDict.TryGetValue(group.MFilesId, out var existing))
                {
                    if (group.Name != existing.Name)
                    {
                        existing.Name = group.Name;
                        _dbContext.MFilesGroups.Update(existing);
                    }
                    
                }
                else
                {
                    _dbContext.MFilesGroups.Add(group);
                }
            }

            var toRemove = existingGroups
                .Where(g => !incomingDict.ContainsKey(g.MFilesId))
                .ToList();
            _dbContext.MFilesGroups.RemoveRange(toRemove);

            await _dbContext.SaveChangesAsync();

        }

        public async Task<List<MFilesGroup>> GetGroupsAsync(int serverId)
        {
            return await _dbContext.MFilesGroups
                .Include(g => g.Accounts)
                .Where(g => g.ServerId == serverId)
                .ToListAsync();
        }
    }
}
