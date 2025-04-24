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

        public async Task SyncLinks(int serverId, int mfilesGroupId, HashSet<string> accountNames)
        {
            var group = _dbContext.MFilesGroups
                .Include(g => g.Accounts)
                .FirstOrDefault(g => g.ServerId == serverId && g.MFilesId == mfilesGroupId);

            var accountDict = await _dbContext.MFilesAccounts.Where(a=>a.ServerId == serverId).ToDictionaryAsync(a => a.AccountName, a => a);

            var dbMembers = group.Accounts.Select(a => a.AccountName).ToHashSet();


            var accountsToAdd = accountNames.Except(dbMembers);
            foreach (var accountName in accountsToAdd)
            {
                if (accountDict.TryGetValue(accountName, out var account))
                {
                    group.Accounts.Add(account);
                }
            }

            

            var accountsToRemove = dbMembers.Except(accountNames);
            foreach (var accountName in accountsToRemove)
            {
                var account = group.Accounts.FirstOrDefault(a => a.AccountName == accountName);
                if (account != null)
                {
                    group.Accounts.Remove(account);
                }
            }

            _dbContext.MFilesGroups.Update(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SyncLinks(int serverId, int mfilesGroupId, HashSet<int> userIds)
        {
            var group = _dbContext.MFilesGroups
                .Include(g => g.Accounts)
                .FirstOrDefault(g => g.ServerId == serverId && g.MFilesId == mfilesGroupId);

            var accountDict = await _dbContext.MFilesAccounts.Where(a => a.ServerId == serverId && a.UserId != 0).ToDictionaryAsync(a => a.UserId, a => a);

            var dbMembers = group.Accounts.Select(a => a.UserId).ToHashSet();


            var accountsToAdd = userIds.Except(dbMembers);
            foreach (var accUserId in accountsToAdd)
            {
                if (accountDict.TryGetValue(accUserId, out var account))
                {
                    group.Accounts.Add(account);
                }
            }



            var accountsToRemove = dbMembers.Except(userIds);
            foreach (var accUsrId in accountsToRemove)
            {
                var account = group.Accounts.FirstOrDefault(a => a.UserId == accUsrId);
                if (account != null)
                {
                    group.Accounts.Remove(account);
                }
            }

            _dbContext.MFilesGroups.Update(group);
            await _dbContext.SaveChangesAsync();
        }
    }
}
