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

        public UserAccount GetUserAccountFromLoginAccountName(int mfServerId,string accountName)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfServerId));
            UserAccounts searchResults = connector.Vault.UserOperationsEx.SearchForUserAccount(accountName);
            if (searchResults.Count == 0)
            {
                throw new ArgumentException();
            }
            return searchResults[1];
        }

        public LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfilesServerLocalId));
            return connector.Vault.UserOperations.GetLoginAccountOfUser(userId);
        }

        public bool ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense)
        {
            using var connector = _connectorFactory.CreateConnection(_credentialRepo.GetConnexionInfos(mfilesServerId));

            LoginAccount targetedAccount;
            ServerLoginAccountOperations lAccountOperator = connector.ServerApplication.LoginAccountOperations;

            try
            {
                targetedAccount = lAccountOperator.GetLoginAccount(accountName);
                targetedAccount.LicenseType = newLicense;
                lAccountOperator.ModifyLoginAccount(targetedAccount);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
