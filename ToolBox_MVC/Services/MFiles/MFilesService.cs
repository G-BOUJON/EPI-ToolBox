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
        
        private readonly IMFilesConnexionInfosService _connexionInfoService;
        private readonly IMFilesConnectorFactory _connectorFactory;


        public MFilesService(IMFilesConnexionInfosService connexionService, IMFilesConnectorFactory connectorFactory)
        {
            
            _connexionInfoService = connexionService;
            _connectorFactory = connectorFactory;
        }

        public LoginAccounts GetLoginAccounts(int mfilesServerLocalId)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfilesServerLocalId));
            return connector.ServerApplication.LoginAccountOperations.GetLoginAccounts();
        }

        public UserGroups GetUserGroups(int mfilesServerLocalId)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfilesServerLocalId));
            return connector.Vault.UserGroupOperations.GetUserGroups();
        }

        public UserAccounts GetUserAccounts(int mfServerId)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfServerId));
            return connector.Vault.UserOperations.GetUserAccounts();
        }

        public LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfilesServerLocalId));
            return connector.Vault.UserOperations.GetLoginAccountOfUser(userId);
        }

        public bool ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfilesServerId));

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

        public void ChangeAccountStatus(int mfServerId, int userId, bool activeStatus)
        {
            using var connector = _connectorFactory.CreateConnection(_connexionInfoService.GetConnexionInfos(mfServerId));

            VaultUserOperations userOperations = connector.Vault.UserOperations;
            ServerLoginAccountOperations loginOperations = connector.ServerApplication.LoginAccountOperations;

            try
            {
                UserAccount userAcc = userOperations.GetUserAccount(userId);
                LoginAccount loginAcc = loginOperations.GetLoginAccount(userAcc.LoginName);

                userAcc.Enabled = activeStatus;
                loginAcc.Enabled = activeStatus;

                userOperations.ModifyUserAccount(userAcc);
                loginOperations.ModifyLoginAccount(loginAcc);
            }
            catch (Exception)
            {
                throw new Exception("Error during status change");
            }
        }
    }
}
