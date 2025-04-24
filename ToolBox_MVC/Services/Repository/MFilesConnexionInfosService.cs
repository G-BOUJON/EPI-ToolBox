using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.Repository
{
    public class MFilesConnexionInfosService : IMFilesConnexionInfosService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly IMFilesServerRepository _serverRepository;

        public MFilesConnexionInfosService(ICredentialRepository credentialRepository, IMFilesServerRepository serverRepository)
        {
            _credentialRepository = credentialRepository;
            _serverRepository = serverRepository;
        }

        public MFilesConnexionInfo GetConnexionInfos(int serverId)
        {
            var credentials = _credentialRepository.GetCredentials(serverId);
            var server = _serverRepository.GetMFilesServer(serverId);

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
