using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.DB;
using System.Linq;
using MFilesAPI;
using ToolBox_MVC.Services.MFiles;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Areas.LicenseManager.Services
{

    public class LicenseManager : ILicenseMangagerService
    {
        private readonly IAccountRepository _accountsRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IMFilesService _mFilesService;



        public LicenseManager(IAccountRepository accountsRepo, IGroupRepository groupRepository, IMFilesService mFilesService)
        {
            _accountsRepo = accountsRepo;
            _groupRepo = groupRepository;
            _mFilesService = mFilesService;
        }

        public async Task<IEnumerable<MFilesAccount>> GetAccountsToRemoveLicenseAsync(int serverId)
        {
            var allAccounts = await _accountsRepo.GetAllInServerAsync(serverId);
            var licensedAccounts = allAccounts.Where(a => a.License != (int)MFLicenseType.MFLicenseTypeNone).ToHashSet();

            var selectedGroups = (await _groupRepo.GetAllInServerIncludeAccountsAsync(serverId)).Where(g => g.Maintained).ToHashSet();

            var toRemoveAccounts = licensedAccounts.Where(a => !selectedGroups.Any(g => g.Accounts.Contains(a)) || !(a.Enabled && a.Active));

            return toRemoveAccounts;
        }

        public async Task<IEnumerable<MFilesAccount>> GetAccountsToRestoreLicenseAsync(int serverId)
        {
            var allAcounts = await _accountsRepo.GetAllInServerAsync(serverId);
            var unlicensedAccounts = allAcounts.Where(a => a.License == (int)MFLicenseType.MFLicenseTypeNone).ToHashSet();

            var selectedGroups = (await _groupRepo.GetAllInServerIncludeAccountsAsync(serverId)).Where(g => g.Maintained).ToHashSet();

            var toRestoreAccounts = unlicensedAccounts.Where(a => selectedGroups.Any(g => g.Accounts.Contains(a)) && a.Active && a.Enabled && !string.IsNullOrEmpty(a.EmailAddress));

            return toRestoreAccounts;
        }

        public async Task RemoveLicenseAsync(int serverId, string accountName)
        {
            var account = await _accountsRepo.GetByAccountNameAsync(serverId, accountName);
            if (account == null || account.Maintained)
            {
                return;
            }

            if (await _mFilesService.ChangeAccountLicense(serverId, accountName, MFLicenseType.MFLicenseTypeNone))
            {
                
                account.License = (int)MFLicenseType.MFLicenseTypeNone;
                await _accountsRepo.SaveChangesAsync();

                // gestion historique
            }
            else
            {
                // gestion erreur
                Console.WriteLine("Erreur lors de la suppression de licence");
            }
        }

        public async Task RestoreLicenseAsync(int serverId, string accountName)
        {
            var account = await _accountsRepo.GetByAccountNameAsync(serverId, accountName);
            if (account == null || account.Maintained)
            {
                return;
            }

            if (await _mFilesService.ChangeAccountLicense(serverId, accountName, MFLicenseType.MFLicenseTypeReadOnlyLicense)) 
            {
                
                account.License = (int)MFLicenseType.MFLicenseTypeReadOnlyLicense;
                await _accountsRepo.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Erreur lors de la restauration de licence");
            }
        }

        public async void MaintainAccount(int accountId)
        {
            var accountToMaintain = await _accountsRepo.GetByIDAsync(accountId);
            ArgumentNullException.ThrowIfNull(accountToMaintain);
            accountToMaintain.Maintained = true;
            await _accountsRepo.SaveChangesAsync();
        }

        public async void UnmaintainAccount(int accountId)
        {
            var accountToMaintain = await _accountsRepo.GetByIDAsync(accountId);
            accountToMaintain.Maintained = false;
            await _accountsRepo.SaveChangesAsync();
        }

        
    }
}
