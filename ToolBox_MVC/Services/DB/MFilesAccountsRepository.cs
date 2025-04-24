using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;
using ToolBox_MVC.Services.DB;
using EFCore.BulkExtensions;

namespace ToolBox_MVC.Services.DB
{
    public class MFilesAccountsRepository : IAccountsRepository
    {
        private readonly ToolBoxDbContext _dbContext;

        public MFilesAccountsRepository(ToolBoxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Compte n'a pas besoin de MAJ si seul critère de change est "Maintained"
        private bool AccountNeedsUpdate(MFilesAccount oldAccount, MFilesAccount newAccount)
        {
            return oldAccount.License != newAccount.License
                || oldAccount.AccountType != newAccount.AccountType
                || oldAccount.EmailAddress != newAccount.EmailAddress
                || oldAccount.FullName != newAccount.FullName
                || oldAccount.UserName != newAccount.UserName
                || oldAccount.ServerRole != newAccount.ServerRole
                || oldAccount.Enabled != newAccount.Enabled
                || oldAccount.Active != newAccount.Active
                || oldAccount.Domain != newAccount.Domain;
        }


        // Pas de changement à "Mainained" ou aux clés primaires ("AccountName","ServerId")
        private MFilesAccount UpdateAccount(MFilesAccount oldAccount, MFilesAccount newAccount)
        {
            newAccount.Maintained = oldAccount.Maintained;
            newAccount.Id = oldAccount.Id;
            newAccount.AccountName = oldAccount.AccountName;
            newAccount.ServerId = oldAccount.ServerId;

            return newAccount;
        }


        

        /// <summary>
        /// Synchronize DB with incoming Accounts List
        /// </summary>
        /// <param name="serverId">ID of the server which is targeted by the sync</param>
        /// <param name="currentAccounts">List of incoming accounts</param>
        /// <returns></returns>
        public async Task SyncAccountsAsync(int serverId, List<MFilesAccount> currentAccounts)
        {
            var existingUsers = await GetUsersAsync(serverId);

            var existingDict = existingUsers.ToDictionary(a => a.AccountName, a => a);
            var incomingDict = currentAccounts.ToDictionary(a => a.AccountName, a => a);

            foreach (var account in currentAccounts)
            {
                if (existingDict.TryGetValue(account.AccountName, out var existing))
                {
                    // Mise à jour si différences
                    if (AccountNeedsUpdate(existing, account))
                    {
                        existing = UpdateAccount(existing, account);
                        _dbContext.MFilesAccounts.Update(existing);
                        
                    }
                    
                    
                }
                // Sinon ajout
                else
                {
                    _dbContext.MFilesAccounts.Add(account);
                }
            }

            var toRemove = existingUsers
                .Where(a => !incomingDict.ContainsKey(a.AccountName))
                .ToList();

            _dbContext.MFilesAccounts.RemoveRange(toRemove);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SyncAccountsBatchAsync(int serverId, HashSet<MFilesAccount> accountsBatch)
        {
            var existingAccounts = await _dbContext.MFilesAccounts
                .Where(a => a.ServerId == serverId && accountsBatch.Select(c => c.AccountName).Contains(a.AccountName))
                .ToHashSetAsync();

            var accountsToUpdate = new HashSet<MFilesAccount>();
            var accountsToInsert = new HashSet<MFilesAccount>();

            foreach (var account in accountsBatch)
            {
                var existingAccount = existingAccounts.FirstOrDefault(a => a.AccountName == account.AccountName);
                if (existingAccount == null)
                {
                    accountsToInsert.Add(account);
                }
                else
                {
                    accountsToUpdate.Add(UpdateAccount(existingAccount, account));
                }
            }

            await _dbContext.BulkInsertAsync(accountsToInsert);
            await _dbContext.BulkUpdateAsync(accountsToUpdate);

            accountsToInsert.Clear();
            accountsToUpdate.Clear();

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAccountsNotInSyncAsync(int serverId, HashSet<string> accountNamesSaw)
        {
            var accountsToDelete = await _dbContext.MFilesAccounts
                .Where(a => a.ServerId == serverId && !accountNamesSaw.Contains(a.AccountName))
                .ToListAsync();

            if (accountsToDelete.Count > 0)
            {
                _dbContext.MFilesAccounts.RemoveRange(accountsToDelete);
                await _dbContext.SaveChangesAsync();
            }

            accountsToDelete.Clear();
        }

        /// <summary>
        /// Fetch all accounts on the DB with the selected serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<HashSet<MFilesAccount>> GetUsersAsync(int serverId)
        {
            var accounts = await _dbContext.MFilesAccounts
                .Where(a => a.ServerId == serverId)
                .ToHashSetAsync();

            return accounts;
        }

        public MFilesAccount GetAccount(int serverId, string accountName)
        {
            MFilesAccount? account = _dbContext.MFilesAccounts.FirstOrDefault(a => a.ServerId == serverId && a.AccountName == accountName);

            if (account == null)
            {
                throw new ArgumentNullException(accountName, "Ce compte n'existe pas dans la BDD");
            }

            return account;
        }

        public async Task UpdateOrAddAccount(MFilesAccount account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var allAccounts = await GetUsersAsync(account.ServerId);

            var accountsDict = allAccounts.ToDictionary(a => a.AccountName, a => a);

            if (accountsDict.TryGetValue(account.AccountName, out var existingAccount))
            {
                if (AccountNeedsUpdate(existingAccount, account))
                {
                    _dbContext.MFilesAccounts.Update(UpdateAccount(existingAccount, account));
                }
            }
            else
            {
                _dbContext.MFilesAccounts.Add(account);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
