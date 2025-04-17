using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesSyncService : ISyncService
    {
        private readonly IMFilesService _mfilesService;
        private readonly IAccountsRepository _accountsRepository;
        private readonly IActiveDirectoryUsersHandler _activeDirectoryUsersHandler;
        private readonly IGroupRepository _groupRepo;
        private readonly IGroupAccountRepository _groupAccountRepo;

        public MFilesSyncService(IMFilesService mfilesService, IAccountsRepository accountsRepository, IActiveDirectoryUsersHandler activeDirectoryUsersHandler, IGroupRepository groupRepo, IGroupAccountRepository groupAccountRepo)
        {
            _mfilesService = mfilesService;
            _accountsRepository = accountsRepository;
            _activeDirectoryUsersHandler = activeDirectoryUsersHandler;
            _groupRepo = groupRepo;
            _groupAccountRepo = groupAccountRepo;
        }

        public async Task SyncAccountsAsync(int serverId)
        {
            LoginAccounts incomingLoginAccounts = _mfilesService.GetAllAccounts(serverId);
            List<MFilesAccount> mFilesAccounts = new List<MFilesAccount>();

            foreach (LoginAccount loginAccount in incomingLoginAccounts)
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
                    Active = false
                };
                try
                {
                    account.Active = _activeDirectoryUsersHandler.IsUserActive(account.UserName);

                }
                catch (ArgumentNullException ex)
                {
                    account.Active = false;
                }
                
                mFilesAccounts.Add(account);
            }

            await _accountsRepository.SyncAccountsAsync(serverId, mFilesAccounts);
        }

        public async Task SyncGroupsAsync(int serverId)
        {
            UserGroups incomingGroups = _mfilesService.GetAllGroups(serverId);
            List<MFilesGroup> mFilesGroups = new List<MFilesGroup>();

            foreach (UserGroup userGroup in incomingGroups)
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
            UserGroups mfilesGroups = _mfilesService.GetAllGroups(serverId);
            List<string> accountNames = new List<string>();
            string singleAccountName;
           

            foreach (UserGroup userGroup in mfilesGroups)
            {
                if (userGroup.Predefined)
                {
                    continue;
                }


                accountNames.Clear();

                foreach(int userId in userGroup.Members)
                {
                    if (userId > 0)
                    {
                        singleAccountName = GetAccountNameFromId(serverId, userId);
                        accountNames.Add(singleAccountName);
                    }
                }
                
                
                await _groupAccountRepo.SyncLinks(serverId, userGroup.ID, accountNames);
                
            }
        }

        private string GetAccountNameFromId(int serverId, int userId)
        {
            return _mfilesService.GetUserSpecificLoginAccount(serverId, userId).AccountName;
        }
    }
}
