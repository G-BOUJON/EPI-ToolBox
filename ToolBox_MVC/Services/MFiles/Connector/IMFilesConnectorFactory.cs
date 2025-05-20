using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Services.MFiles.Connector
{
    public interface IMFilesConnectorFactory
    {
        IMFilesConnector CreateConnection(int serverId);
    }

    public class MFConnectorFactory : IMFilesConnectorFactory
    {
        private readonly List<MFilesServer> Servers;

        public MFConnectorFactory(IMfCredentialStore credStore , IServerRepository serverRepo) 
        {
            var allServers = Task.Run(() => serverRepo.GetAllAsync()).Result;
            Servers = allServers.Select(s => s.Clone()).ToList();
            foreach (var server in Servers)
            {
                server.MfCredential = Task.Run(() => credStore.GetCredentials(server.Id)).Result;
            }
        }

        public IMFilesConnector CreateConnection(int serverId)
        {
            return new MFilesConnector(Servers.First(s=> s.Id == serverId));
        }
    }
}
