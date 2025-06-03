using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services.MFiles
{
    public interface IMfilesAccountActivationHandler
    {
        Task<IEnumerable<MFilesAccount>> GetAccountsToReactivateAsync(int serverID);

        /// <summary>
        /// Modifiy an account's status on a M-Files server
        /// </summary>
        /// <param name="serverID">The ID of the M-Files server on which to operate</param>
        /// <param name="mfUserID">The UserID of the targeted UserAccount on M-Files</param>
        /// <param name="activeStatus">The targeted status</param>
        Task ModifyMFilesAccountStatus(int serverID, int mfUserID, bool activeStatus, bool automatic = false);
    }
}