using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IADCredentialService
    {
        Task<ADCredential> GetCredential(int serverID);
        Task UpdateCredentials(int serverID, ADCredential credential);
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

        public async Task<ADCredential> GetCredential(int serverID)
        {
            var server = await _serverRepo.GetByIDAsync(serverID);
            ArgumentNullException.ThrowIfNull(server);

            var cred = new ADCredential(
                server.ADCredential.Domain,
                server.ADCredential.Container,
                _dataProtector.Unprotect(server.ADCredential.EncryptedUsername),
                _dataProtector.Unprotect(server.ADCredential.EncryptedPassword));

            return cred;
        }

        public async Task UpdateCredentials(int serverID, ADCredential credential)
        {
            var server = await _serverRepo.GetByIDAsync(serverID);
            ArgumentNullException.ThrowIfNull(server);

            credential.EncryptedUsername = _dataProtector.Protect(credential.EncryptedUsername);
            credential.EncryptedPassword = _dataProtector.Protect(credential.EncryptedPassword);

            server.ADCredential = credential;
            await _serverRepo.SaveChangesAsync();
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
