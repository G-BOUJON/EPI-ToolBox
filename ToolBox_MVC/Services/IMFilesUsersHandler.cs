using MFilesAPI;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public interface IMFilesUsersHandler
    {
        MFilesServerApplication MFilesServerApp { get; set; } 
        Config Configuration { get; set; }

        List<LoginAccount> GetSuppressionList();
        List<LoginAccount> GetRestorationList();
        void DeleteAccountLicence(string accountName);
        void RestoreAccountLicense(string accountName);
        bool GroupExists(string groupName);
        bool AreValidCredentials(string username, string password);
    }
}
