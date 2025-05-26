using MFilesAPI;
using System.Runtime.InteropServices;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.MFiles.Connector
{
    public interface IMFilesConnector : IDisposable
    {
        MFilesServerApplication ServerApplication { get; }
        Vault Vault { get; }

        MfConnexionResult ConnectionResult { get; }


    }

    public class MFilesConnector : IMFilesConnector
    {
        private bool disposedValue;

        public MFilesServerApplication ServerApplication { get; }

        public Vault Vault { get; set; }

        public MfConnexionResult ConnectionResult { get; }

        public MFilesConnector(MFilesServer connexionInfo)
        {
            ServerApplication = new MFilesServerApplication();

            try
            {
                var connectionResult = ServerApplication.Connect(AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                    UserName: connexionInfo.MfCredential.EncryptedUserName,
                    Password: connexionInfo.MfCredential.EncryptedPassword,
                    Domain: connexionInfo.Domain,
                    ProtocolSequence: connexionInfo.ProtocolSequence,
                    NetworkAddress: connexionInfo.NetworkAddress,
                    Endpoint: connexionInfo.EndPoint,
                    AllowAnonymousConnection: true
                    );

                if (connectionResult == MFServerConnection.MFServerConnectionAnonymous)
                {
                    Vault = new Vault();
                    ConnectionResult = MfConnexionResult.IncorrectCredentials;
                }
                else
                {
                    try
                    {
                        Vault = ServerApplication.LogInToVault(connexionInfo.VaultGuid);
                        ConnectionResult = MfConnexionResult.Success;
                    }
                    catch (COMException)
                    {
                        Vault = new Vault();
                        ConnectionResult = MfConnexionResult.IncorrectVaultGUID;
                    }
                }
            }
            catch (COMException)
            {
                Vault = new Vault();
                ConnectionResult = MfConnexionResult.IncorrectInfos;
            }

            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                ServerApplication.Disconnect();
                Vault = null;

                disposedValue = true;
            }
        }

        

        public void Dispose()
        {
            
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public enum MfConnexionResult
    {
        Success = 0,
        IncorrectCredentials = 1,
        IncorrectVaultGUID = 2,
        IncorrectInfos = 3
    }
}
