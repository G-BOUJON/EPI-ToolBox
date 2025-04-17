using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;
using ToolBox_MVC.Services.DB;

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
            oldAccount.License = newAccount.License;
            oldAccount.AccountType = newAccount.AccountType;
            oldAccount.EmailAddress = newAccount.EmailAddress;
            oldAccount.FullName = newAccount.FullName;
            oldAccount.UserName = newAccount.UserName;
            oldAccount.ServerRole = newAccount.ServerRole;
            oldAccount.Enabled = newAccount.Enabled;
            oldAccount.Enabled = newAccount.Enabled;
            oldAccount.Active = newAccount.Active;
            oldAccount.Domain = newAccount.Domain;

            return oldAccount;
        }


        /// <summary>
        /// Synchronize DB with incoming Accounts List
        /// </summary>
        /// <param name="serverId">ID of the server which is targeted by the sync</param>
        /// <param name="currentAccounts">List of incoming accounts</param>
        /// <returns></returns>
        public async Task SyncAccountsAsync(int serverId, List<MFilesAccount> currentAccounts)
        {
            var existingUsers = await GetUserAsync(serverId);

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

        /// <summary>
        /// Fetch all accounts on the DB with the selected serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public async Task<List<MFilesAccount>> GetUserAsync(int serverId)
        {
            List<MFilesAccount> accounts = await _dbContext.MFilesAccounts
                .Where(a => a.ServerId == serverId)
                .ToListAsync();

            return accounts;
        }
    }
}
