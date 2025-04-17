using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.Repository
{
    public interface ICredentialRepository
    {
        Credentials GetCredentials(int mfilesServerId);
        void UpdateCredentials(Credentials credentials, int mfilesServerId);
        MFilesConnexionInfo GetConnexionInfos(int mfilesServerId);
    }
}
