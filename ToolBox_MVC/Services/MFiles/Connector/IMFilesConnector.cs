﻿using MFilesAPI;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.MFiles.Connector
{
    public interface IMFilesConnector : IDisposable
    {
        MFilesServerApplication ServerApplication { get; }
        Vault Vault { get; }


    }

    public class MFilesConnector : IMFilesConnector
    {
        private bool disposedValue;

        public MFilesServerApplication ServerApplication { get; }

        public Vault? Vault { get; set; }

        public MFilesConnector(MFilesConnexionInfo connexionInfo)
        {
            ServerApplication = new MFilesServerApplication();
            ServerApplication.Connect(AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: connexionInfo.Username,
                Password: connexionInfo.Password,
                Domain: connexionInfo.Domain,
                ProtocolSequence: connexionInfo.ProtocolSequence,
                NetworkAddress: connexionInfo.NetworkAddress,
                Endpoint: connexionInfo.EndPoint
                );

            Vault = ServerApplication.LogInToVault(connexionInfo.VaultGuid);
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
}
