using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IADCredentialService
    {
        
        void ProtectCredentials(Models.ActiveDirectory ad);
        void UnprotectCredentials(Models.ActiveDirectory ad);
        
    }

    public class ADCredentialStore : IADCredentialService
    {
        private readonly IServerRepository _serverRepo;
        private readonly IDataProtector _dataProtector;

        public ADCredentialStore(IServerRepository serverRepo, IDataProtectionProvider dataProtectionProvider)
        {
            _serverRepo = serverRepo;
            _dataProtector = dataProtectionProvider.CreateProtector("AD.Credential");
        }

        

        

        public void ProtectCredentials(Models.ActiveDirectory ad)
        {
            ad.EncryptedCredentials.Username = _dataProtector.Protect(ad.EncryptedCredentials.Username);
            ad.EncryptedCredentials.Password = _dataProtector.Protect(ad.EncryptedCredentials.Password);
        }

        public void UnprotectCredentials(Models.ActiveDirectory ad)
        {
            ad.EncryptedCredentials.Username = _dataProtector.Unprotect(ad.EncryptedCredentials.Username);
            ad.EncryptedCredentials.Password = _dataProtector.Unprotect(ad.EncryptedCredentials.Password);
        }
    }

    public class ADConnexionInfos
    {
        public string Domain { get; set; }
        public string Container { get; set; }
        public string Username {  get; set; }
        public string Password { get; set; }

        public ADConnexionInfos() { }
    }

}
