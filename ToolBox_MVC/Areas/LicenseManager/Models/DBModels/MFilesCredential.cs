using Microsoft.EntityFrameworkCore;

namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    [Owned]
    public class MFilesCredential
    {      
        public string EncryptedUserName { get; set; }
        public string EncryptedPassword { get; set; }
        
        

        public MFilesCredential() { }
    }
}
