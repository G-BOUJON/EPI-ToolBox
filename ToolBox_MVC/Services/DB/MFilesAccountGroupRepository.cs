using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using ToolBox.Models;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Services.DB
{
    public class MFilesAccountGroupRepository : IGroupAccountRepository
    {
        private readonly ToolBoxDbContext _dbContext;
       

        public MFilesAccountGroupRepository(ToolBoxDbContext dbContext, IAccountsRepository accountRepo)
        {
            _dbContext = dbContext;
            
        }

        public async Task SyncAccountGroupLink(int serverId, int mfilesGroupId, string accountName)
        {
            var account = _dbContext.MFilesAccounts.FirstOrDefault(a => a.ServerId == serverId && a.AccountName == accountName);
            var group = _dbContext.MFilesGroups
                .Include(g => g.Accounts)
                .FirstOrDefault(g => g.ServerId == serverId && g.MFilesId == mfilesGroupId);

            if (account == null || group == null) 
            {
                throw new ArgumentNullException("Ce compte ou groupe n'existe pas");
            }

            if (!group.Accounts.Contains(account))
            {
                group.Accounts.Add(account);
                _dbContext.MFilesGroups.Update(group);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task SyncLinks(int serverId, int mfilesGroupId, List<string> accountNames)
        {
            var group = _dbContext.MFilesGroups
                .Include(g => g.Accounts)
                .FirstOrDefault(g => g.ServerId == serverId && g.MFilesId == mfilesGroupId);

            var accountsToRemove = await _dbContext.MFilesAccounts.Where(a => !accountNames.Contains(a.AccountName) && a.ServerId == serverId).ToListAsync();
            var accountsToAdd = await _dbContext.MFilesAccounts
                .Where(a => accountNames.Contains(a.AccountName) && a.ServerId == serverId && !group.Accounts.Contains(a))
                .ToListAsync();

            group.Accounts.AddRange(accountsToAdd);
            
            foreach (var account in accountsToRemove)
            {
                group.Accounts.Remove(account);
            }

            _dbContext.MFilesGroups.Update(group);
            await _dbContext.SaveChangesAsync();
        }
    }
}
