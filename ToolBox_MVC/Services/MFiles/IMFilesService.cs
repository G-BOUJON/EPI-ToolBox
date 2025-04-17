using MFilesAPI;

namespace ToolBox_MVC.Services.MFiles
{
    public interface IMFilesService
    {
        LoginAccounts GetAllAccounts(int mfilesServerLocalId);
        UserGroups GetAllGroups(int mfilesServerLocalId);
        LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId);
    }
}
