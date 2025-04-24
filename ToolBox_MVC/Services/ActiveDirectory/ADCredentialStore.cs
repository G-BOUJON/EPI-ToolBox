using Microsoft.AspNetCore.DataProtection;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IADCredRepository
    {
        ADConnexionInfos GetADCredential(int adId);
        void AddOrUpdateCredential(ADConnexionInfos newCred);
    }

    public class ADCredentialStore : IADCredRepository
    {
        private readonly ToolBoxDbContext _dbContext;
        private readonly IDataProtector _dataProtector;

        public ADCredentialStore(ToolBoxDbContext dbContext, IDataProtectionProvider dataProtectionProvider)
        {
            _dbContext = dbContext;
            _dataProtector = dataProtectionProvider.CreateProtector("AD.Credential");
        }

        public ADConnexionInfos GetADCredential(int adId)
        {
            var cred = _dbContext.ADCredentials.FirstOrDefault(c => c.Id == adId);

            ArgumentNullException.ThrowIfNull(cred);

            return new ADConnexionInfos()
            {
                Domain = cred.Domain,
                Container = cred.Container,
                Username = _dataProtector.Unprotect(cred.EncryptedUsername),
                Password = _dataProtector.Unprotect(cred.EncryptedPassword)
            };
        }

        public void AddOrUpdateCredential(ADConnexionInfos newCred)
        {
            var oldCred = _dbContext.ADCredentials.FirstOrDefault(c => c.Domain == newCred.Domain);

            if (oldCred != null)
            {
                oldCred.Container = newCred.Container;
                oldCred.EncryptedUsername = _dataProtector.Protect(newCred.Username);
                oldCred.EncryptedPassword = _dataProtector.Protect(newCred.Password);

                _dbContext.ADCredentials.Update(oldCred);
            }
            else
            {
                _dbContext.ADCredentials.Add(new ADCredential()
                {
                    Domain = newCred.Domain,
                    Container = newCred.Container,
                    EncryptedUsername = _dataProtector.Protect(newCred.Username),
                    EncryptedPassword = _dataProtector.Protect(newCred.Password)
                });
            }

            _dbContext.SaveChanges();

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
