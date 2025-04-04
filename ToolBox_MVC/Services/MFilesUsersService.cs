using MFilesAPI;
using Microsoft.AspNetCore.Authentication;
using NuGet.Packaging;
using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public class MFilesUsersService
    {
        public MFilesServerApplication mfServerApplication { get; set; }
        public Vault vault { get; set; }
        public PrincipalContext pc { get; set; }
        public ServerType serverType { get; set; }
        public Config Configuration { get; set; }
        public MFilesUsersService(Config config)
        {
            Configuration = config;
            mfServerApplication = new MFilesServerApplication();
            mfServerApplication.Connect(
                AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: config.VaultCredentials.Username,
                Password: config.VaultCredentials.Password,
                Domain: config.VaultCredentials.Domain,
                ProtocolSequence: config.VaultCredentials.ProtocolSequence,
                NetworkAddress: config.VaultCredentials.NetworkAddress,
                Endpoint: config.VaultCredentials.EndPoint);
            vault = mfServerApplication.LogInToVault(config.VaultCredentials.Guid);

            pc = new(contextType: ContextType.Domain, name: config.ActiveDirectoryCredentials.Domain, container: config.ActiveDirectoryCredentials.Container, userName: config.ActiveDirectoryCredentials.Username, password: config.ActiveDirectoryCredentials.Password);
        }

        public List<LoginAccount> GetSuppressionList()
        {
            // Initialisation
            List<Group> groupNames = Configuration.Groups.ToList();
            List<LoginAccount> nonExistingAccounts = new List<LoginAccount>();

            LoginAccounts loginAccounts = getLoginAccounts();
            List<LoginAccount> licencedAccounts = getLicencedAccounts(loginAccounts);
            List<LoginAccount> windowsAccounts = getWindowsAccounts(licencedAccounts);

            // Traitement
            List<GroupPrincipal> groups = new List<GroupPrincipal>();
            foreach (Group groupName in groupNames)
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName.name);
                groups.AddRange(getAllSubGroups(group));
            }

            int iteration = 1;
            string numberOfAccounts = Convert.ToString(windowsAccounts.Count);
            foreach (LoginAccount account in windowsAccounts)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(Convert.ToString(iteration) + " comptes traités sur " + numberOfAccounts);
                if (!recursiveIsExistingUser(groups, account.UserName))
                {
                    nonExistingAccounts.Add(account);
                }

                iteration += 1;
            }
            Console.WriteLine();

            // Sortie
            return nonExistingAccounts;
        }

        public List<UserGroup> GetMFilesUserGroups()
        {
            VaultUserGroupOperations groupOperations = vault.UserGroupOperations;
            
            List<UserGroup> groupsToInspect = new List<UserGroup>();

            foreach (UserGroup userGroup in groupOperations.GetUserGroups())
            {
                if (Configuration.GetGroupsNames().Contains(userGroup.Name))
                {
                    groupsToInspect.Add(userGroup);
                }
            }

            return groupsToInspect;
        }

        public List<LoginAccount> GetRestorationList()
        {
            List<LoginAccount> accountsToRestore = new List<LoginAccount>();
            LoginAccount currentAccount = null;

            List<UserGroup> mfGroups = GetMFilesUserGroups();


            foreach (UserGroup userGroup in mfGroups)
            {
                foreach (int userID in userGroup.Members)
                {
                    // Dans la collection Members, les id de groupes sont négatifs et les id d'utilisateur positifs
                    if (userID > 0)
                    {
                        currentAccount = GetLoginAccountFromUserAccountID(userID);
                        if (currentAccount != null)
                        {
                            if(currentAccount.LicenseType == MFLicenseType.MFLicenseTypeNone)
                            {
                                accountsToRestore.Add(currentAccount);
                            }
                        }

                    }
                }
            }

            return accountsToRestore;
        }

        public LoginAccount GetLoginAccountFromUserAccountID(int userID)
        {
            VaultUserOperations userOperations = vault.UserOperations;
            
            return userOperations.GetLoginAccountOfUser(userID);
        }
        

        public bool groupExists(string name)
        {
            // Initialisation
            bool groupExists = true;

            // Traitement
            GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, name);
            if (group == null)
            {
                groupExists = false;
            }

            // Sortie
            return groupExists;
        }

        LoginAccounts getLoginAccounts()
        {
            // Initialisation
            LoginAccounts logins = new LoginAccounts();
            ServerLoginAccountOperations loginAccountOps = mfServerApplication.LoginAccountOperations;

            // Traitement   
            logins = loginAccountOps.GetLoginAccounts();

            // Sortie
            return logins;
        }

        UserAccounts getUserAccounts()
        {
            // Initialisation
            UserAccounts? users = null;
            VaultUserOperations userOps = vault.UserOperations;

            // Traitement
            users = userOps.GetUserAccounts();

            // Sortie
            return users;
        }

        bool changeLicence(LoginAccount account, MFLicenseType licence)
        {
            // Initialisation
            bool success = true;
            ServerLoginAccountOperations loginAccountOps = mfServerApplication.LoginAccountOperations;

            // Traitement
            try
            {
                account.LicenseType = licence;
                loginAccountOps.ModifyLoginAccount(account);
            }
            catch (Exception e)
            {
                success = false;
            }

            // Sortie
            return success;
        }

        public LoginAccount convertToLoginAccount(Account account)
        {
            return mfServerApplication.LoginAccountOperations.GetLoginAccount(account.AccountName);
        }

        /// <summary>
        /// Removes the license of a M-Files login account that is not in the maintained account list
        /// </summary>
        /// <param name="account">The login account whose license to remove</param>
        /// <returns>>True if operations was succeful (license was removed or account is protected), false otherwise</returns>
        public bool DeleteLicense(LoginAccount account)
        {
            ServerLoginAccountOperations loginOperations = mfServerApplication.LoginAccountOperations;
            bool success = true;

            if (!Configuration.MaintainedAccounts.Contains(account.UserName))
            {
                try
                {
                    new JsonHistoryService(serverType).addDeletedAccount(new Account(account));
                    account.LicenseType = MFLicenseType.MFLicenseTypeNone;
                    loginOperations.ModifyLoginAccount(account);
                }
                catch (Exception e)
                {
                    success = false;
                }
            }
            return success;
        }


        /// <summary>
        /// Removes the license of a M-Files login account that is not in the maintained account list
        /// </summary>
        /// <param name="accountName">The name of the account to remove</param>
        /// <returns>True if operations was succeful (license was removed or account is protected), false otherwise</returns>
        public bool DeleteLicense(string accountName)
        {
            LoginAccount account = mfServerApplication.LoginAccountOperations.GetLoginAccount(accountName);
            return DeleteLicense(account);
        }

        List<LoginAccount> getLicencedAccounts(LoginAccounts loginAccounts)
        {
            // Initialisation
            List<LoginAccount> licencedAccounts = new List<LoginAccount>();

            // Traitement
            foreach (LoginAccount account in loginAccounts)
            {
                if (account.LicenseType != MFLicenseType.MFLicenseTypeNone)
                {
                    licencedAccounts.Add(account);
                }
            }

            // Sortie
            return licencedAccounts;
        }


        List<LoginAccount> getWindowsAccounts(List<LoginAccount> loginAccounts)
        {
            // Initialisation
            List<LoginAccount> windowsAccounts = new List<LoginAccount>();

            // Traitement
            foreach (LoginAccount account in loginAccounts)
            {
                if (account.AccountType == MFLoginAccountType.MFLoginAccountTypeWindows)
                {
                    windowsAccounts.Add(account);
                }
            }


            // Sortie
            return windowsAccounts;
        }


        // Gestion de l'Active Directory

        public bool areValidCredentials(string username, string password)
        {
            // Initialisation
            bool validCredentials = true;

            // Traitement
            if (!pc.ValidateCredentials(username, password))
            {
                validCredentials = false;
            }

            // Sortie
            return validCredentials;
        }

        bool isExistingUser(GroupPrincipal group, string username)
        {
            // Initialisation
            bool userExists = false;
            Principal user = Principal.FindByIdentity(pc, IdentityType.Name, username);

            // Traitement
            if (user != null)
            {

                if (user.IsMemberOf(group))
                {
                    userExists = true;
                }
            }

            // Sortie
            return userExists;
        }

        List<GroupPrincipal> getAllSubGroups(GroupPrincipal group)
        {
            // Initialisation
            List<GroupPrincipal> subGroups = new List<GroupPrincipal>();

            // Traitement
            subGroups.Add(group);

            foreach (Principal pr in group.GetMembers())
            {
                GroupPrincipal subgroup = GroupPrincipal.FindByIdentity(pc, pr.Name);
                if (subgroup != null)
                {
                    subGroups.Add(subgroup);
                    foreach (GroupPrincipal groupPrincipal in getAllSubGroups(subgroup))
                    {
                        subGroups.Add(groupPrincipal);
                    }
                }
            }

            // Sortie
            return subGroups;
        }

        bool recursiveIsExistingUser(List<GroupPrincipal> groups, string username)
        {
            // Initialisation
            UserPrincipal user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, username);
            bool existing = false;

            // Traitement
            if (user != null)
            {
                foreach (GroupPrincipal grp in groups)
                {
                    if (user.IsMemberOf(grp))
                    {
                        existing = true; break;
                    }
                }
            }

            // Sortie
            return existing;
        }
    }
}
