using MFilesAPI;
using System.Configuration;
using System.Threading.Tasks;
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

        public async Task<LoginAccounts> GetLoginAccounts(int mfilesServerLocalId)
        {
            using var connector = await CreateConnector(mfilesServerLocalId);
            return connector.ServerApplication.LoginAccountOperations.GetLoginAccounts();
        }

        public async Task<UserGroups> GetUserGroups(int mfilesServerLocalId)
        {
            using var connector = await CreateConnector(mfilesServerLocalId);
            return connector.Vault.UserGroupOperations.GetUserGroups();
        }

        public async Task<UserAccounts> GetUserAccounts(int mfServerId)
        {
            using var connector = await CreateConnector(mfServerId);
            return connector.Vault.UserOperations.GetUserAccounts();
        }

        public async Task<LoginAccount> GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId)
        {
            using var connector = await CreateConnector(mfilesServerLocalId);
            return connector.Vault.UserOperations.GetLoginAccountOfUser(userId);
        }

        public async Task<bool> ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense)
        {
            using var connector = await CreateConnector(mfilesServerId);

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

        public async Task ChangeAccountStatus(int mfServerId, int userId, bool activeStatus)
        {
            using var connector = await CreateConnector(mfServerId);

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

        private async Task<IMFilesConnector> CreateConnector(int serverID)
        {
            return _connectorFactory.CreateConnection(await _connexionInfoService.GetConnexionInfos(serverID));
        }
    }
}
