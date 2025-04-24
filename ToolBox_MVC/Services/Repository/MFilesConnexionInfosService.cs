using ToolBox_MVC.Models;
using ToolBox_MVC.Services.DB;

namespace ToolBox_MVC.Services.Repository
{
    public class MFilesConnexionInfosService : IMFilesConnexionInfosService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly IMfilesServerRepository _serverRepository;

        public MFilesConnexionInfosService(ICredentialRepository credentialRepository, IMfilesServerRepository serverRepository)
        {
            _credentialRepository = credentialRepository;
            _serverRepository = serverRepository;
        }

        public MFilesConnexionInfo GetConnexionInfos(int serverId)
        {
            var credentials = _credentialRepository.GetCredentials(serverId);
            var server = _serverRepository.GetServerInfos(serverId);

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
}
