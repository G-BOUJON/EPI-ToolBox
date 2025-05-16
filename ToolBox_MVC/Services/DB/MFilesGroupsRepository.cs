using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Services.DB
{
    public class MFilesGroupsRepository : IGroupRepositoryold
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

        public async Task<MFilesGroup> GetGroupAsync(int groupID)
        {
            var group = await _dbContext.MFilesGroups.FindAsync(groupID);
            ArgumentNullException.ThrowIfNull(group);
            return group;
        }

        public async Task<MFilesGroup> GetGroupAsync(int serverID, string groupName)
        {
            var group = await _dbContext.MFilesGroups.FirstOrDefaultAsync(g => g.ServerId == serverID && g.Name == groupName);
            ArgumentNullException.ThrowIfNull(group);
            return group;
        }

        public async Task UpdateOrAddGroupAsync(MFilesGroup group)
        {
            try
            {
                MFilesGroup dbGroup;
                if (group.ServerId != null)
                {
                    dbGroup = await GetGroupAsync((int)group.ServerId, group.Name);
                }
                else
                {
                    dbGroup = await GetGroupAsync(group.Id);
                }
                _dbContext.MFilesGroups.Update(group);
            }
            catch (ArgumentNullException)
            {
                _dbContext.MFilesGroups.Add(group);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
