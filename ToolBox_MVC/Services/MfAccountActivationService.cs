using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.DB;
using ToolBox_MVC.Services.MFiles;

namespace ToolBox_MVC.Services
{
    public class MfAccountActivationService : IMfilesAccountActivationHandler
    {
        private IMFilesService _mFilesService;
        private IAccountRepository _mfAccountsRepo;

        public MfAccountActivationService(IMFilesService mFilesService, IAccountRepository mfAccountsRepo)
        {
            _mFilesService = mFilesService;
            _mfAccountsRepo = mfAccountsRepo;
        }

        public async Task<IEnumerable<MFilesAccount>> GetAllAccountsToModify(int serverID)
        {
            var allAccounts = await _mfAccountsRepo.GetAllInServerAsync(serverID);

            var accountsToModify = allAccounts.Where(a => a.Active != a.Enabled);

            return accountsToModify;
        }

        public void ModifyMFilesAccountStatus(int serverID, int mfUserID, bool activeStatus)
        {
            _mFilesService.ChangeAccountStatus(serverID, mfUserID, activeStatus);
        }

        public async Task ModifyAllIncorrectAccounts(int serverID)
        {
            var accountsToModify = await GetAllAccountsToModify(serverID);

            foreach (var account in accountsToModify)
            {
                try
                {
                    ModifyMFilesAccountStatus(serverID,account.UserId,account.Active);

                    account.Enabled = account.Active;
                }
                catch (Exception)
                {
                    // Message erreur ou traitement d'erreur
                }
            }

            await _mfAccountsRepo.SaveChangesAsync();

        }
    }
}
