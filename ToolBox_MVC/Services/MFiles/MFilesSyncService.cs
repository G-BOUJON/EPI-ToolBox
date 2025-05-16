using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles.Sync;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesSyncService : ISyncService
    {
        private readonly IMFilesService _mfilesService;
        private readonly IMfilesServerRepository _serverRepo;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAdService _adService;
        private readonly IGroupRepositoryold _groupRepo;
        private readonly IGroupAccountRepository _groupAccountRepo;

        public MFilesSyncService(IMFilesService mfilesService, IAccountsRepository accountsRepository,IMfilesServerRepository mFilesServerRepository, IAdService activeDirectoryUsersHandler, IGroupRepositoryold groupRepo, IGroupAccountRepository groupAccountRepo)
        {
            _mfilesService = mfilesService;
            _accountsRepository = accountsRepository;
            _adService = activeDirectoryUsersHandler;
            _groupRepo = groupRepo;
            _groupAccountRepo = groupAccountRepo;
            _serverRepo = mFilesServerRepository;
        }

        public async Task SyncAccountsAsync(int serverId)
        {
            const int batchSize = 100;
            HashSet<MFilesAccount> batch = new HashSet<MFilesAccount>(batchSize);
            int adLocalId = 1;

            var sawedNames = new HashSet<string>();

            foreach (LoginAccount loginAccount in await _mfilesService.GetLoginAccounts(serverId))
            {
                var account = new MFilesAccount
                {
                    ServerId = serverId,
                    AccountName = loginAccount.AccountName,
                    AccountType = (int)loginAccount.AccountType,
                    Domain = loginAccount.DomainName,
                    EmailAddress = loginAccount.EmailAddress,
                    Enabled = loginAccount.Enabled,
                    FullName = loginAccount.FullName,
                    License = (int)loginAccount.LicenseType,
                    ServerRole = (int)loginAccount.ServerRoles,
                    UserName = loginAccount.UserName,
                    Maintained = false,
                    Active = true
                };
                if (loginAccount.AccountType == MFLoginAccountType.MFLoginAccountTypeWindows)
                {
                    try
                    {
                        account.Active = _adService.IsUserActive(adLocalId, account.UserName);

                    }
                    catch (ArgumentNullException)
                    {
                        account.Active = false;
                    }
                }
                
                
                batch.Add(account);
                sawedNames.Add(account.AccountName);

                if (batch.Count >= batchSize)
                {
                    await _accountsRepository.SyncAccountsBatchAsync(serverId,batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await _accountsRepository.SyncAccountsBatchAsync(serverId, batch);
                batch.Clear();
            }

            

            await _accountsRepository.DeleteAccountsNotInSyncAsync(serverId, sawedNames);

            sawedNames.Clear();
        }

        public async Task SyncUserAccountAsync(int serverId)
        {
            foreach (UserAccount userAccount in await _mfilesService.GetUserAccounts(serverId))
            {
                var mfAccount = _accountsRepository.GetAccount(serverId, userAccount.LoginName);
                if (mfAccount != null)
                {
                    mfAccount.UserId = userAccount.ID;
                    await _accountsRepository.UpdateOrAddAccount(mfAccount);
                }
            }
        }

        public async Task SyncGroupsAsync(int serverId)
        {
            List<MFilesGroup> mFilesGroups = new List<MFilesGroup>();

            foreach (UserGroup userGroup in await _mfilesService.GetUserGroups(serverId))
            {
                if (userGroup.Predefined)
                {
                    continue;
                }
                var group = new MFilesGroup
                {
                    ServerId = serverId,
                    Name = userGroup.Name,
                    MFilesId = userGroup.ID
                };
                mFilesGroups.Add(group);
            }

            await _groupRepo.SyncGroupsAsync(serverId, mFilesGroups);
        }

        public async Task SyncGroupsAccountsLinksAsync(int serverId)
        {
            
            var userIds = new HashSet<int>();
           

            foreach (UserGroup userGroup in await _mfilesService.GetUserGroups(serverId))
            {
                if (userGroup.Predefined)
                {
                    continue;
                }


                

                foreach(int userId in userGroup.Members)
                {
                    if (userId > 0)
                    {
                        
                        userIds.Add(userId);
                    }
                }
                
                
                await _groupAccountRepo.SyncLinks(serverId, userGroup.ID, userIds);
                userIds.Clear();
            }
        }
    }
}
