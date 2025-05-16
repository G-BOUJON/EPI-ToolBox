using MFilesAPI;

namespace ToolBox_MVC.Services.MFiles
{
    public interface IMFilesService
    {
        Task<LoginAccounts> GetLoginAccounts(int mfilesServerLocalId);
        Task<UserGroups> GetUserGroups(int mfilesServerLocalId);
        Task<LoginAccount> GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId);

        Task<bool> ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense);
        
        Task<UserAccounts> GetUserAccounts(int mfServerId);

        Task ChangeAccountStatus(int mfServerId, int userId, bool activeStatus);
    }
}
