/*
 *      Auteur :    Gabriel BOUJON
 *      Date :      2025-04-11
 * 
 *      Desc. :     Classe service pour la gestion des comptes de Login sur le/les serveurs M-Files
 */

using MFilesAPI;
using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesAccountService : IMFilesUsersHandler
    {
        public MFilesServerApplication MFilesServerApp { get; set; }
        public Config Configuration { get; set; }
        public Vault Vault { get; set; }
        public ServerType Server { get; set; }
        public string Domain { get; set; }
        public IActiveDirectoryUsersHandler ADUserHandler { get; set; }
        public List<Group> Groups { get; set; }

        public MFilesAccountService(IConfigurationHandler configHandler, IADUsersHandlerFactory adFactory)
        {
            Config config = configHandler.GetConfiguration();
            Configuration = config;

            VaultCredentials credentials = config.VaultCredentials;
            Server = configHandler.Server;

            MFilesServerApp = new MFilesServerApplication();
            MFilesServerApp.Connect(
                AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: credentials.Username,
                Password: credentials.Password,
                Domain: credentials.Domain,
                ProtocolSequence: credentials.ProtocolSequence,
                NetworkAddress: credentials.NetworkAddress,
                Endpoint: credentials.EndPoint
                );
            Vault = MFilesServerApp.LogInToVault(credentials.Guid);
            Domain = credentials.Domain;

            ADUserHandler = adFactory.Create(config.ActiveDirectoryCredentials);

            Groups = config.Groups;
        }

        public bool AreValidCredentials(string username, string password)
        {
            return ADUserHandler.AreValidCredentials(username, password);
        }

        public List<LoginAccount> GetAllAccounts()
        {
            List<LoginAccount> accounts = new List<LoginAccount>();
            List<LoginAccount> accountsToDelete = GetSuppressionList();
            List<LoginAccount> accountsToRestore = GetRestorationList();

            accounts.AddRange(accountsToDelete);
            accounts.AddRange(accountsToRestore);

            return accounts;
        }

        

        


        public void DeleteAccountLicense(string accountName)
        {
            LoginAccount account = MFilesServerApp.LoginAccountOperations.GetLoginAccount(accountName);
            DeleteAccountLicense(account);
        }

        public void DeleteAccountLicense(LoginAccount account)
        {
            ChangeAccountLicense(account, MFLicenseType.MFLicenseTypeNone);
        }

        public List<LoginAccount> GetRestorationList()
        {
            List<LoginAccount> accountsToRestore = new List<LoginAccount>();

            LoginAccount currentAccount = null;

            List<UserGroup> mfGroups = GetMFilesGroups();

            foreach (UserGroup userGroup in mfGroups)
            {
                foreach (int userID in userGroup.Members)
                {
                    // Dans la collection Members, les id de groupes sont négatifs et les id d'utilisateur positifs
                    if (userID > 0)
                    {
                        currentAccount = GetLoginAccountFromUserAccountID(userID);

                        try
                        {
                            if (currentAccount.LicenseType == MFLicenseType.MFLicenseTypeNone
                                && currentAccount.Enabled && !string.IsNullOrEmpty(currentAccount.EmailAddress)
                                && ADUserHandler.IsUserActive(currentAccount.UserName) && !accountsToRestore.Contains(currentAccount))
                            {
                                accountsToRestore.Add(currentAccount);
                            }
                        }
                        catch (ArgumentNullException ex)
                        {
                            Console.WriteLine(string.Format("Erreur : Le compte {0} n'existe pas dans l'AD", currentAccount.AccountName));
                        }
                    }
                }
            }

            return accountsToRestore;
        }

        public List<LoginAccount> GetSuppressionList()
        {
            VaultUserOperations userOperations = Vault.UserOperations;
            ServerLoginAccountOperations loginOperations = MFilesServerApp.LoginAccountOperations;

            List<LoginAccount> accountsToSuppress = new List<LoginAccount>();
            List<int> membersID = GetAllMembersID(GetMFilesGroups());

            LoginAccount currentAccount;

            foreach(UserAccount user in userOperations.GetUserAccounts())
            {
                currentAccount = loginOperations.GetLoginAccount(user.LoginName);
                if (currentAccount.AccountType != MFLoginAccountType.MFLoginAccountTypeMFiles)
                {

                    try
                    {
                        if (currentAccount.LicenseType != MFLicenseType.MFLicenseTypeNone
                            && (!membersID.Contains(user.ID) || !ADUserHandler.IsUserActive(currentAccount.UserName)))
                        {
                            accountsToSuppress.Add(currentAccount);
                        }
                    }
                    catch (ArgumentNullException ex)
                    {

                        accountsToSuppress.Add(currentAccount);

                    }
                }
            }

            return accountsToSuppress;

        }

        public bool GroupExists(string groupName)
        {
            return ADUserHandler.GroupExists(groupName);
        }

        public void RestoreAccountLicense(LoginAccount account)
        {
            ChangeAccountLicense(account, MFLicenseType.MFLicenseTypeReadOnlyLicense);
        }

        public void RestoreAccountLicense(string accountName)
        {
            LoginAccount account = MFilesServerApp.LoginAccountOperations.GetLoginAccount(accountName);
            RestoreAccountLicense(account);
        }

        public void ChangeAccountLicense(LoginAccount account, MFLicenseType licenseType)
        {
            ServerLoginAccountOperations loginOperations = MFilesServerApp.LoginAccountOperations;
            if (true)
            {
                account.LicenseType = licenseType;
                loginOperations.ModifyLoginAccount(account);
            }
        }

        

        string GetMFilesGroupName(Group group)
        {
            return Domain + "\\" + group.name;
        }

        List<string> GetMFilesGroupNames()
        {
            List<string> groupNames = new List<string>();
            foreach (Group group in Groups)
            {
                groupNames.Add(GetMFilesGroupName(group));
            }
            return groupNames;
        }

        bool MFilesGroupExits(Group group)
        {
            UserGroups userGroups = Vault.UserGroupOperations.GetUserGroups();
            foreach (UserGroup userGroup in userGroups)
            {
                if (userGroup.Name == group.name
                    || userGroup.Name == GetMFilesGroupName(group))
                {
                    return true;
                }
            }
            return false;
        }

        List<UserGroup> GetMFilesGroups()
        {
            VaultUserGroupOperations groupOperations = Vault.UserGroupOperations;


            List<UserGroup> groupsToInspect = new List<UserGroup>();

            foreach (UserGroup userGroup in groupOperations.GetUserGroups())
            {
                if (GetMFilesGroupNames().Contains(userGroup.Name))
                {
                    groupsToInspect.Add(userGroup);
                }
            }

            return groupsToInspect;
        }

        public LoginAccount GetLoginAccountFromUserAccountID(int userID)
        {
            VaultUserOperations userOperations = Vault.UserOperations;

            return userOperations.GetLoginAccountOfUser(userID);
        }

        List<int> GetAllMembersID(List<UserGroup> userGroups)
        {
            List<int> membersID = new List<int>();
            foreach (UserGroup userGroup in userGroups)
            {
                foreach(int id  in userGroup.Members)
                {
                    membersID.Add(id);
                }
            }
            return membersID;
        }
    }
}
