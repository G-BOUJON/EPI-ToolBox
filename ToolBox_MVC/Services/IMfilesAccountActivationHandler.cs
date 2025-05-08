using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services
{
    public interface IMfilesAccountActivationHandler
    {
        /// <summary>
        /// Get all accounts whose status in the AD doesn't match the MFiles status
        /// </summary>
        /// <param name="serverID">The ID of the server to inspect</param>
        /// <returns>A container of all the fetched accounts</returns>
        Task<IEnumerable<MFilesAccount>> GetAllAccountsToModify(int serverID);
        
        /// <summary>
        /// Modifiy an account's status on a M-Files server
        /// </summary>
        /// <param name="serverID">The ID of the M-Files server on which to operate</param>
        /// <param name="mfUserID">The UserID of the targeted UserAccount on M-Files</param>
        /// <param name="activeStatus">The targeted status</param>
        void ModifyMFilesAccountStatus(int serverID, int mfUserID, bool activeStatus);
    }
}