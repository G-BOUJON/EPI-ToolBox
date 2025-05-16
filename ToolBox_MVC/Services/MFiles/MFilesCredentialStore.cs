using Microsoft.AspNetCore.DataProtection;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesCredentialStore : IMfCredentialStore
    {
        private readonly IServerRepository _serverRepo;
        private readonly IDataProtector _dataProtector;

        public MFilesCredentialStore(IServerRepository serverRepository, IDataProtectionProvider dataProtectionpProvider)
        {
            _serverRepo = serverRepository;
            _dataProtector = dataProtectionpProvider.CreateProtector("MFiles.Credentials");
        }

        public async Task<MFilesCredential> GetCredentials(int mfilesServerId)
        {
            var server = await _serverRepo.GetByIDAsync(mfilesServerId);
            ArgumentNullException.ThrowIfNull(server);

            var cred = new MFilesCredential
            {
                EncryptedPassword = _dataProtector.Unprotect(server.MfCredential.EncryptedPassword),
                EncryptedUserName = _dataProtector.Unprotect(server.MfCredential.EncryptedUserName)
            };

            return cred;

        }

        public async Task UpdateCredentials(int mfilesServerId, MFilesCredential credentials)
        {
            var server = await _serverRepo.GetByIDAsync(mfilesServerId);
            ArgumentNullException.ThrowIfNull(server);

            credentials.EncryptedUserName = _dataProtector.Protect(credentials.EncryptedUserName);
            credentials.EncryptedPassword = _dataProtector.Protect(credentials.EncryptedPassword);

            server.MfCredential = credentials;
            await _serverRepo.SaveChangesAsync();
        }
    }
}
