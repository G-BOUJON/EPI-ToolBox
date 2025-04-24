using MFilesAPI;

namespace ToolBox_MVC.Services.MFiles
{
    public interface IMFilesService
    {
        LoginAccounts GetAllAccounts(int mfilesServerLocalId);
        UserGroups GetAllGroups(int mfilesServerLocalId);
        LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId);

        public bool ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense);
        UserAccount GetUserAccountFromLoginAccountName(int mfServerId, string accountName);
    }
}
