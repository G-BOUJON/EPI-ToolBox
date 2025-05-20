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
        
        
        private readonly IMFilesConnectorFactory _connectorFactory;


        public MFilesService(IMFilesConnectorFactory connectorFactory)
        {
            _connectorFactory = connectorFactory;
        }

        public LoginAccounts GetLoginAccounts(int mfilesServerLocalId)
        {
            using var connector = CreateConnector(mfilesServerLocalId);
            return connector.ServerApplication.LoginAccountOperations.GetLoginAccounts();
        }

        public UserGroups GetUserGroups(int mfilesServerLocalId)
        {
            using var connector = CreateConnector(mfilesServerLocalId);
            return connector.Vault.UserGroupOperations.GetUserGroups();
        }

        public UserAccounts GetUserAccounts(int mfServerId)
        {
            using var connector = CreateConnector(mfServerId);
            return connector.Vault.UserOperations.GetUserAccounts();
        }

        public LoginAccount GetUserSpecificLoginAccount(int mfilesServerLocalId, int userId)
        {
            using var connector = CreateConnector(mfilesServerLocalId);
            return connector.Vault.UserOperations.GetLoginAccountOfUser(userId);
        }

        public bool ChangeAccountLicense(int mfilesServerId, string accountName, MFLicenseType newLicense)
        {
            using var connector = CreateConnector(mfilesServerId);

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
            using var connector = CreateConnector(mfServerId);

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

        private IMFilesConnector CreateConnector(int serverID)
        {
            return _connectorFactory.CreateConnection(serverID);
        }
    }
}
