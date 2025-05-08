using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles;

namespace ToolBox_MVC.Services
{
    public class MfAccountActivationService : IMfilesAccountActivationHandler
    {
        private IMFilesService _mFilesService;
        private IAccountsRepository _mfAccountsRepo;

        public MfAccountActivationService(IMFilesService mFilesService, IAccountsRepository mfAccountsRepo)
        {
            _mFilesService = mFilesService;
            _mfAccountsRepo = mfAccountsRepo;
        }

        public async Task<IEnumerable<MFilesAccount>> GetAllAccountsToModify(int serverID)
        {
            var allAccounts = await _mfAccountsRepo.GetUsersAsync(serverID);

            var accountsToModify = allAccounts.Where(a => a.Active != a.Enabled);

            return accountsToModify;
        }

        public void ModifyMFilesAccountStatus(int serverID, int mfUserID, bool activeStatus)
        {
            _mFilesService.ChangeAccountStatus(serverID, mfUserID, activeStatus);
        }
    }
}
