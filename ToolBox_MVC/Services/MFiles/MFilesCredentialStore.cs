using Microsoft.AspNetCore.DataProtection;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesCredentialStore : ICredentialRepository
    {
        private readonly ToolBoxDbContext _context;
        private readonly IDataProtector _dataProtector;

        public MFilesCredentialStore(ToolBoxDbContext context, IDataProtectionProvider dataProtectionpProvider)
        {
            _context = context;
            _dataProtector = dataProtectionpProvider.CreateProtector("MFiles.Credentials");
        }

        public Credentials GetCredentials(int mfilesServerId)
        {
            var cred = _context.MFilesCredentials.FirstOrDefault(c => c.ServerId == mfilesServerId);
            if (cred == null)
            {
                throw new KeyNotFoundException("Pas de credentials pour ce serveur");
            }

            string uncryptedName = _dataProtector.Unprotect(cred.EncryptedUserName);
            string uncryptedPass = _dataProtector.Unprotect(cred.EncryptedPassword);

            return new Credentials(uncryptedName, uncryptedPass, cred.Domain);
        }

        public void UpdateCredentials(Credentials credentials, int mfilesServerId)
        {
            var encryptedUser = _dataProtector.Protect(credentials.Username);
            var encryptedPass = _dataProtector.Protect(credentials.Password);

            var cred = _context.MFilesCredentials.FirstOrDefault(c => c.ServerId == mfilesServerId);
            if (cred == null)
            {
                cred = new MFilesCredential
                {
                    EncryptedUserName = encryptedUser,
                    EncryptedPassword = encryptedPass,
                    Domain = credentials.Domain,
                    ServerId = mfilesServerId
                };
                _context.MFilesCredentials.Add(cred);
            }
            else
            {
                cred.EncryptedUserName = encryptedUser;
                cred.EncryptedPassword = encryptedPass;
                cred.Domain = credentials.Domain;
                _context.Update(cred);
            }

            _context.SaveChanges();
        }

        public virtual MFilesConnexionInfo GetConnexionInfos(int serverId)
        {
            Credentials credentials = GetCredentials(serverId);
            MFilesServer server = _context.MFilesServers.FirstOrDefault(s => s.Id == serverId);

            return new MFilesConnexionInfo
            {
                Username = credentials.Username,
                Password = credentials.Password,
                Domain = credentials.Domain,
                ProtocolSequence = server.ProtocolSequence,
                NetworkAddress = server.NetworkAddress,
                EndPoint = server.EndPoint,
                VaultGuid = server.VaultGuid
            };
        }
    }

    public class CredentialStoreTest : MFilesCredentialStore
    {
        public CredentialStoreTest(ToolBoxDbContext context, IDataProtectionProvider dataProtectionpProvider) : base(context, dataProtectionpProvider)
        {
        }

        public override MFilesConnexionInfo GetConnexionInfos(int serverId)
        {
            return new MFilesConnexionInfo
            {
                Username = "intratest01",
                Password = "Epi2020.",
                Domain = "EPI",
                ProtocolSequence = "ncacn_ip_tcp",
                NetworkAddress = "mfiles.epi.ge.ch",
                EndPoint = "2266",
                VaultGuid = "{D6B60CC3-9531-417A-8819-AFBD4E37DABF}"
            };
        }
    }
}
