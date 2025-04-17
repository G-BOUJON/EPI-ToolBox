using MFilesAPI;
using System.Configuration;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.MFiles.Connector;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesService : IMFilesService
    {
        
        private readonly ICredentialRepository _credentialRepo;
        private readonly IMFilesConnectorFactory _connectorFactory;


        public MFilesService(ICredentialRepository credentialRepo, IMFilesConnectorFactory connectorFactory)
        {
            
            _credentialRepo = credentialRepo;
            _connectorFactory = connectorFactory;
        }

        public LoginAccounts GetAllAccounts(int mfilesServerLocalId)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfilesServerLocalId));
            return connector.ServerApplication.LoginAccountOperations.GetLoginAccounts();
        }

        public UserGroups GetAllGroups(int mfilesServerLocalId)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfilesServerLocalId));
            return connector.Vault.UserGroupOperations.GetUserGroups();
        }

        public LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfilesServerLocalId));
            return connector.Vault.UserOperations.GetLoginAccountOfUser(userId);
        }
        

        
    }
}
