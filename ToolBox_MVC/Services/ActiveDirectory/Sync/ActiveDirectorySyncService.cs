using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Services.ActiveDirectory.Sync
{
    public class ActiveDirectorySyncService : IActiveDirectorySyncService
    {
        private readonly IGenericRepository<Models.ActiveDirectory> _adRepo;
        private readonly IADAccountRepository _accountRepo;
        private readonly IADGroupRepository _groupRepo;
        private readonly IAdService _adService;


        public ActiveDirectorySyncService(IGenericRepository<Models.ActiveDirectory> adRepo, IADAccountRepository accountRepo, IADGroupRepository groupRepo, IAdService adService)
        {
            _adRepo = adRepo;
            _accountRepo = accountRepo;
            _groupRepo = groupRepo;
            _adService = adService;
        }

        public ADAccount ProcessUserPrincipal(Models.ActiveDirectory ad, UserPrincipal user)
        {
            var processedAccount = new ADAccount(user.Guid.ToString(), user.SamAccountName, user.EmailAddress, user.DisplayName, (bool)user.Enabled);
            processedAccount.ActiveDirectory = ad;
            return processedAccount;
        }

        public Task<ADAccount> ProcessUserPrincipalAsync(Models.ActiveDirectory ad, UserPrincipal user)
        {
            return Task.Run(() => ProcessUserPrincipal(ad, user));
        }

        public ADGroup ProcessGroupPrincipal(Models.ActiveDirectory ad, GroupPrincipal group, Dictionary<string, ADAccount> accountDict)
        {
            var processedGroup = new ADGroup(group.Guid.ToString(), group.Name);
            processedGroup.ActiveDirectory = ad;
            processedGroup.Accounts = new List<ADAccount>();


            foreach (var principal in group.Members)
            {
                if (accountDict.TryGetValue(principal.Guid.ToString(), out var account))
                {
                    processedGroup.Accounts.Add(account);
                }
            }

            return processedGroup;
        }

        public Task<ADGroup> ProcessGroupPrincipalAsync(Models.ActiveDirectory ad, GroupPrincipal group, Dictionary<string, ADAccount> accountDict)
        {
            return Task.Run(() => ProcessGroupPrincipal(ad, group, accountDict));
        }


        public async Task SyncADAccountAsync(int adID)
        {
            var incomingUserPrincipal = _adService.GetUserPrincipals(adID);
            var ad = await _adRepo.GetByIDAsync(adID);
            var existingAccounts = await _accountRepo.GetAllInActiveDirectoryAsync(adID);

            Dictionary<string, ADAccount> existingDict = existingAccounts.ToDictionary(a => a.GUID);

            var processTask = new List<Task<ADAccount>>();

            foreach (var user in incomingUserPrincipal)
            {
                processTask.Add(ProcessUserPrincipalAsync(ad, user));
            }

            await Task.WhenAll(processTask);

            var processedAccounts = processTask.Select(t => t.Result);

            var toAdd = new List<ADAccount>();
            var toUpdate = new List<(ADAccount Existing, ADAccount Incoming)>();
            var incomingGUID = new HashSet<string>(processedAccounts.Select(a => a.GUID));

            foreach (var account in processedAccounts)
            {
                if (existingDict.TryGetValue(account.GUID, out var existing))
                {
                    toUpdate.Add((existing, account));
                }
                else
                {
                    toAdd.Add(account);
                }
            }

            var toDelete = existingAccounts.Where(a => !incomingGUID.Contains(a.GUID));


            foreach (var adding in toAdd)
            {
                await _accountRepo.AddAsync(adding);
            }

            foreach (var (existing, incoming) in toUpdate)
            {
                existing.Name = incoming.Name;
                existing.EmailAdress = incoming.EmailAdress;
                existing.Enabled = incoming.Enabled;
                existing.DisplayName = incoming.DisplayName;
            }

            foreach (var deleting in toDelete)
            {
                _accountRepo.Delete(deleting);
            }

            await _accountRepo.SaveChangesAsync();
        }

        public async Task SyncADGroupAsync(int adID)
        {
            var incomingGroupPrincipal = _adService.GetGroupPrincipals(adID);
            var ad = await _adRepo.GetByIDAsync(adID);
            var existingGroups = await _groupRepo.GetAllInActiveDirectoryAsync(adID);

            var accounts = await _accountRepo.GetAllInActiveDirectoryAsync(adID);
            Dictionary<string, ADAccount> accountDict = accounts.ToDictionary(a => a.GUID);


            Dictionary<string, ADGroup> existingDict = existingGroups.ToDictionary(a => a.GUID);

            var processTask = new List<Task<ADGroup>>();

            foreach (var group in incomingGroupPrincipal)
            {
                processTask.Add(ProcessGroupPrincipalAsync(ad, group, accountDict));
            }


            incomingGroupPrincipal.Clear();
            accounts.Clear();
            

            await Task.WhenAll(processTask);

            var processedGroups = processTask.Select(t => t.Result);

            

            var toAdd = new List<ADGroup>();
            var toUpdate = new List<(ADGroup Existing, ADGroup Incoming)>();
            var incomingGUID = new HashSet<string>(processedGroups.Select(a => a.GUID));

            foreach (var group in processedGroups)
            {
                if (existingDict.TryGetValue(group.GUID, out var existing))
                {
                    toUpdate.Add((existing, group));
                }
                else
                {
                    toAdd.Add(group);
                }
            }

            processedGroups = null;

            var toDelete = existingGroups.Where(a => !incomingGUID.Contains(a.GUID));


            foreach (var adding in toAdd)
            {
                await _groupRepo.AddAsync(adding);
            }

            foreach (var (existing, incoming) in toUpdate)
            {
                existing.Name = incoming.Name;
                existing.Accounts = incoming.Accounts;
            }

            foreach (var deleting in toDelete)
            {
                _groupRepo.Delete(deleting);
            }

            await _groupRepo.SaveChangesAsync();
        }

        public async Task SyncActiveDirectory(int activeDirectoryID)
        {
            await SyncADAccountAsync(activeDirectoryID);
            await SyncADGroupAsync(activeDirectoryID);

            var activeDir = await _adRepo.GetByIDAsync(activeDirectoryID);
            activeDir.LastSync = DateOnly.FromDateTime(DateTime.Now);
            await _adRepo.SaveChangesAsync();
        }
    }
}
