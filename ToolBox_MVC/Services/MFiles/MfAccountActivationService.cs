using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Areas.LicenseManager.Services;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory;

namespace ToolBox_MVC.Services.MFiles
{
    public class MfAccountActivationService : IMfilesAccountActivationHandler
    {
        private IMFilesService _mFilesService;
        private IAccountRepository _mfAccountsRepo;
        private IGroupRepository _groupRepo;
        private IADAccountRepository _adAccountRepository;
        private IADGroupRepository _aDGroupRepository;
        private IOperationHistoryService _operationHistoryService;

        public MfAccountActivationService(IMFilesService mFilesService, IAccountRepository mfAccountsRepo, IGroupRepository groupRepo, IADAccountRepository adAccountRepository, IADGroupRepository aDGroupRepository, IOperationHistoryService operationHistoryService)
        {
            _mFilesService = mFilesService;
            _mfAccountsRepo = mfAccountsRepo;
            _groupRepo = groupRepo;
            
            _adAccountRepository = adAccountRepository;
            _aDGroupRepository = aDGroupRepository;
            _operationHistoryService = operationHistoryService;
        }

        
        

        public async Task<IEnumerable<MFilesAccount>> GetAccountsToReactivateAsync(int serverID)
        {
            var allAccounts = await _mfAccountsRepo.GetAllInServerIncludeAsync(serverID);
            var maintainedGroups = (await _groupRepo.GetAllInServerIncludeAccountsAsync(serverID)).Where(g => g.Maintained);
            List<ADGroup> aDGroups = new List<ADGroup>();

            foreach (var mfGroup in maintainedGroups)
            {
                if (mfGroup.ADGroup != null)
                {
                    aDGroups.Add(await _aDGroupRepository.GetByGUIDAsync(mfGroup.ADGroup.GUID));
                }
            }


            var accountsToReactivate = allAccounts.
                Where(a => a.Active != a.Enabled && a.AccountType == (int)MFLoginAccountType.MFLoginAccountTypeWindows && a.Active == true
                && (maintainedGroups.Any(gm => gm.Accounts.Contains(a)) || aDGroups.Any(ga => ga.Accounts.Contains(a.ADAccount)))
                );

            return accountsToReactivate;
        }

        public async Task ModifyMFilesAccountStatus(int serverID, int mfUserID, bool activeStatus, bool automatic = false)
        {
            _mFilesService.ChangeAccountStatus(serverID, mfUserID, activeStatus);
            var account = await _mfAccountsRepo.GetByMfIDAsync(serverID, mfUserID);
            account.Active = activeStatus;

            await _mfAccountsRepo.SaveChangesAsync();
            await _operationHistoryService.RegisterNewOperation(account, OperationType.Activation, automatic);
        }
    }
}
