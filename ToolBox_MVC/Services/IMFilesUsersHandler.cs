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
        void DeleteAccountLicense(string accountName);
        void DeleteAccountLicense(LoginAccount account);

        void RestoreAccountLicense(LoginAccount account);
        void RestoreAccountLicense(string accountName);
        bool GroupExists(string groupName);
        bool AreValidCredentials(string username, string password);
    }
}
