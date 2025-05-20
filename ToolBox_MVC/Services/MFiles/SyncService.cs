using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Services.MFiles
{
    public class SyncService : ISyncService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IAdService _adService;
        private readonly IMFilesService _mFilesService;

        public SyncService(IAccountRepository accountRepository, IGroupRepository groupRepository, IAdService adService, IMFilesService mFilesService)
        {
            _accountRepository = accountRepository;
            _groupRepository = groupRepository;
            _adService = adService;
            _mFilesService = mFilesService;
        }

        private MFilesAccount ProcessLoginAccount(LoginAccount logAccount, int serverId, Dictionary<string,(int id,bool enabled)> userAccounts)
        {
            var processedAccount = new MFilesAccount(logAccount,serverId);

            try
            {
                processedAccount.Active = _adService.IsUserActive(serverId, logAccount.UserName);
            }
            catch (Exception)
            {
                processedAccount.Active = false;
            }

            if (userAccounts.TryGetValue(logAccount.AccountName, out var user))
            {
                processedAccount.UserId = user.id;
                processedAccount.Enabled = user.enabled || processedAccount.Enabled;
            }
            else
            {
                processedAccount.UserId = 0;
            }

            return processedAccount;
        }

        private Task<MFilesAccount> ProcessLoginAccountAsync(LoginAccount logAccount, int serverId, Dictionary<string,(int,bool)> userAccounts)
        {
            return Task.Run(() => ProcessLoginAccount(logAccount, serverId, userAccounts));
        }

        private MFilesGroup ProcessUserGroup(UserGroup usrGroup, int serverId, Dictionary<int, MFilesAccount> accountDict)
        {
            var processedGroup = new MFilesGroup
            {
                Name = usrGroup.Name,
                MFilesId = usrGroup.ID,
                ServerId = serverId,
                Accounts = new List<MFilesAccount>()
            };

            foreach (int memberID in usrGroup.Members)
            {
                if (memberID > 0)
                {
                    if (accountDict.TryGetValue(memberID, out var account))
                    {
                        processedGroup.Accounts.Add(account);
                    }
                }
            }

            return processedGroup;
        }

        private Task<MFilesGroup> ProcessUserGroupAsync(UserGroup usrGroup, int serverId, Dictionary<int, MFilesAccount> accountDict)
        {
            return Task.Run(() => ProcessUserGroup(usrGroup, serverId, accountDict));
        }

        public async Task SyncAccountsAsync(int serverId)
        {

            var incomingLogAccounts = _mFilesService.GetLoginAccounts(serverId);
            var incomingUserAccounts = _mFilesService.GetUserAccounts(serverId);
            var existingAccounts = await _accountRepository.GetAllInServerAsync(serverId);

            Dictionary<string,MFilesAccount> existingByAccountName = existingAccounts.ToDictionary(a => a.AccountName);

            var userAccDict = new Dictionary<string, (int,bool)>();

            foreach (UserAccount user in incomingUserAccounts)
            {
                userAccDict.Add(user.LoginName, (user.ID,user.Enabled));
            }

            var processTaks = new List<Task<MFilesAccount>>();

            foreach (LoginAccount loginAccount in incomingLogAccounts)
            {
                processTaks.Add(ProcessLoginAccountAsync(loginAccount, serverId, userAccDict));
            }

            await Task.WhenAll(processTaks);

            var processedAccounts = processTaks.Select(t => t.Result);

            var toAdd = new List<MFilesAccount>();
            var toUpdate = new List<(MFilesAccount oldAccount, MFilesAccount incomingAccount)>();
            var incomingNames = new HashSet<string>(processedAccounts.Select(a => a.AccountName));

            foreach (var account in processedAccounts)
            {
                if (existingByAccountName.TryGetValue(account.AccountName, out var existing))
                {
                    toUpdate.Add((existing, account));
                }
                else
                {
                    toAdd.Add(account);
                }
            }

            var toDelete = existingAccounts.Where(a => !incomingNames.Contains(a.AccountName));

            // Opérations sur BDD

            foreach (var account in toAdd)
            {
                await _accountRepository.AddAsync(account);
                
            }

            foreach (var (existing, incoming) in toUpdate)
            {
                existing.ServerRole = incoming.ServerRole;
                existing.EmailAddress = incoming.EmailAddress;
                existing.Active = incoming.Active;
                existing.Enabled = incoming.Enabled;
                existing.FullName = incoming.FullName;
                existing.License = incoming.License;
                existing.AccountType = incoming.AccountType;
                existing.UserId = incoming.UserId;
                existing.UserName = incoming.UserName;
                existing.Domain = incoming.Domain;

                
            }

            foreach (var account in toDelete)
            {
                _accountRepository.Delete(account);
                
            }

            await _accountRepository.SaveChangesAsync();
        }

        

        public async Task SyncGroupsAsync(int serverId)
        {
            

            var incomingGroups = _mFilesService.GetUserGroups(serverId);
            var accountDict = (await _accountRepository.GetAllInServerAsync(serverId)).Where(a => a.UserId != 0).ToDictionary(a => a.UserId);
            var existingGroups = await _groupRepository.GetAllInServerIncludeAccountsAsync(serverId);

            Dictionary<int, MFilesGroup> existingByMfID = existingGroups.ToDictionary(g => g.MFilesId);

            var processTasks = new List<Task<MFilesGroup>>();

            foreach (UserGroup group in incomingGroups)
            {
                processTasks.Add(ProcessUserGroupAsync(group, serverId, accountDict));
            }

            await Task.WhenAll(processTasks);

            var processedGroups = processTasks.Select(t => t.Result);

            var toAdd = new List<MFilesGroup>();
            var toUpdate = new List<(MFilesGroup existing, MFilesGroup updated)>();
            var incomingIds = new HashSet<int>(processedGroups.Select(g => g.MFilesId));

            foreach (var group in processedGroups)
            {
                if (existingByMfID.TryGetValue(group.MFilesId, out var existing))
                {
                    toUpdate.Add((existing, group));
                }
                else
                {
                    toAdd.Add(group);
                }
            }

            var toDelete = existingGroups.Where(g => !incomingIds.Contains(g.MFilesId));


            foreach(var group in toAdd)
            {
                await _groupRepository.AddAsync(group);
            }

            foreach(var (existing, updated) in toUpdate)
            {
                existing.Name = updated.Name;
                existing.Accounts = updated.Accounts;
            }

            foreach( var  group in toDelete)
            {
                _groupRepository.Delete(group);
            }

            await _groupRepository.SaveChangesAsync();
        }

        
    }
}
