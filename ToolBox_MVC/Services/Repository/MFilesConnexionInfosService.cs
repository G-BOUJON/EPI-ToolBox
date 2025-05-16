using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.DB;

namespace ToolBox_MVC.Services.Repository
{
    public class MFilesConnexionInfosService : IMFilesConnexionInfosService
    {
        private readonly IMfCredentialStore _credentialRepository;
        private readonly IServerRepository _serverRepository;

        public MFilesConnexionInfosService(IMfCredentialStore credentialRepository, IServerRepository serverRepository)
        {
            _credentialRepository = credentialRepository;
            _serverRepository = serverRepository;
        }

        public async Task<MFilesConnexionInfo> GetConnexionInfos(int serverId)
        {
            var server = await _serverRepository.GetByIDAsync(serverId);
            ArgumentNullException.ThrowIfNull(server);
            var credentials = await _credentialRepository.GetCredentials(serverId);
            
            return new MFilesConnexionInfo
            {
                Username = credentials.EncryptedUserName,
                Password = credentials.EncryptedPassword,
                Domain = server.Domain,
                ProtocolSequence = server.ProtocolSequence,
                NetworkAddress = server.NetworkAddress,
                EndPoint = server.EndPoint,
                VaultGuid = server.VaultGuid
            };
        }
    }
}
