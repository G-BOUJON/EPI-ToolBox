using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.Repository
{
    public interface IMfCredentialStore
    {
        Task<MFilesCredential> GetCredentials(int mfilesServerId);
        Task UpdateCredentials(int mfilesServerId, MFilesCredential credentials);
    }
}
