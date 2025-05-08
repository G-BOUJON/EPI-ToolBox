using MFilesAPI;

namespace ToolBox_MVC.Services.MFiles
{
    public interface IMFilesService
    {
        LoginAccounts GetLoginAccounts(int mfilesServerLocalId);
        UserGroups GetUserGroups(int mfilesServerLocalId);
        LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId);

        bool ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense);
        
        UserAccounts GetUserAccounts(int mfServerId);

        void ChangeAccountStatus(int mfServerId, int userId, bool activeStatus);
    }
}
