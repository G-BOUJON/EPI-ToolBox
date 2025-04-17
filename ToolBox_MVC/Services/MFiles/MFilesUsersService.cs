using MFilesAPI;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text.Json;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.MFiles
{
    public class MFilesUsersService : IMFilesUsersHandler
    {
        public MFilesServerApplication MFilesServerApp { get; set; }
        public Vault Vault { get; set; }
        public PrincipalContext pc { get; set; }
        public ServerType Server { get; set; }
        public Config Configuration { get; set; }
        
        public MFilesUsersService(ServerType server, IConfigurationHandler configurationHandler)
        {
            Configuration = configurationHandler.GetConfiguration();
            MFilesServerApp = new MFilesServerApplication();
            MFilesServerApp.Connect(
                AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: Configuration.VaultCredentials.Username,
                Password: Configuration.VaultCredentials.Password,
                Domain: Configuration.VaultCredentials.Domain,
                ProtocolSequence: Configuration.VaultCredentials.ProtocolSequence,
                NetworkAddress: Configuration.VaultCredentials.NetworkAddress,
                Endpoint: Configuration.VaultCredentials.EndPoint);
            Vault = MFilesServerApp.LogInToVault(Configuration.VaultCredentials.Guid);

            pc = new(contextType: ContextType.Domain, name: Configuration.ActiveDirectoryCredentials.Domain, container: Configuration.ActiveDirectoryCredentials.Container, userName: Configuration.ActiveDirectoryCredentials.Username, password: Configuration.ActiveDirectoryCredentials.Password);
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

        public List<GroupPrincipal> GetGroupPrincipals(List<Group> group)
        {
            List<GroupPrincipal> groups = new List<GroupPrincipal>();
            GroupPrincipal principalGroupCurrent;

            foreach(Group currentGroup in group)
            {
                principalGroupCurrent = GroupPrincipal.FindByIdentity(pc, currentGroup.name);
                groups.AddRange(getAllSubGroups(principalGroupCurrent));
            }

            return groups;
        }

        public List<UserGroup> GetMFilesUserGroups()
        {
            VaultUserGroupOperations groupOperations = Vault.UserGroupOperations;

            
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

        public List<LoginAccount> GetRestorationList_V1()
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
                            if(currentAccount.LicenseType == MFLicenseType.MFLicenseTypeNone && currentAccount.Enabled && !string.IsNullOrEmpty(currentAccount.EmailAddress))
                            {
                                accountsToRestore.Add(currentAccount);
                            }
                        }

                    }
                }
            }

            return accountsToRestore;
        }

        public List<LoginAccount> GetRestorationsListFromAD()
        {
            

            LoginAccount currentAccount = null;
            List<GroupPrincipal> groups = GetGroupPrincipals(Configuration.Groups);

            List<string> usersPrincipals = new List<string>();

            List<LoginAccount> unlicensedAccounts = new List<LoginAccount>();
            UserPrincipal userPrincipal = null;
            bool isActive = false;
            foreach (LoginAccount laccount in MFilesServerApp.LoginAccountOperations.GetLoginAccounts())
            {
                if (laccount.LicenseType == MFLicenseType.MFLicenseTypeNone && laccount.Enabled  || laccount.UserName == "CCASIMO")
                {
                    
                    
                    if (recursiveIsExistingUser(groups, laccount.UserName))
                    {
                        userPrincipal = GetADUser(laccount.UserName);
                        if (userPrincipal != null)
                        {
                            isActive = IsActive(userPrincipal);
                            usersPrincipals.Add(userPrincipal.UserPrincipalName + " - " + isActive.ToString());
                            if (isActive)
                            {
                                unlicensedAccounts.Add(laccount);
                            }
                        }
                        
                    }
                }
            }

            JsonSerializer.Serialize(new Utf8JsonWriter(File.OpenWrite(FilePathService.LicenseManagerPath(ServerType.Prod) + "adAccounts.json"), new JsonWriterOptions
            {
                SkipValidation = true,
                Indented = true
            }),
                    usersPrincipals);

            return unlicensedAccounts;
        }

        public List<LoginAccount> GetRestorationList()
        {
            List<LoginAccount> accountsToRestore = new List<LoginAccount>();

            LoginAccount currentAccount = null;
            UserPrincipal currentADUser = null;

            List<UserGroup> mfGroups = GetMFilesUserGroups();

            foreach (UserGroup userGroup in mfGroups)
            {
                foreach (int userID in userGroup.Members)
                {
                    // Dans la collection Members, les id de groupes sont négatifs et les id d'utilisateur positifs
                    if (userID > 0)
                    {
                        currentAccount = GetLoginAccountFromUserAccountID(userID);
                        currentADUser = GetADUser(currentAccount.UserName);

                        if (currentADUser != null)
                        {
                            if (currentAccount.LicenseType == MFLicenseType.MFLicenseTypeNone && currentAccount.Enabled  && !string.IsNullOrEmpty(currentAccount.EmailAddress) && IsActive(currentADUser))
                            {
                                accountsToRestore.Add(currentAccount);
                            }
                        }
                    }
                }
            }

            return accountsToRestore;
        }

        public List<LoginAccount> GetADUsers()
        {

            using (var searcher = new PrincipalSearcher(new UserPrincipal(pc)))
            {
                foreach (var result in searcher.FindAll())
                {
                    DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                    if (de.Properties["userAccountControl"].Value != null)
                        {
                            int flags = (int)de.Properties["userAccountControl"].Value;

                            Console.WriteLine(de.Properties["sAMAccountName"].Value + " = " + flags.ToString());
                        }
                    
                }
            }

            return new List<LoginAccount>();
        }

        private bool IsActive(UserPrincipal user)
        {
            
            DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;

            if (de.NativeGuid == null)
            {
                return false;
            }

            if (de.Properties["userAccountControl"].Value == null)
            {
                return true;
            }

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        public LoginAccount GetLoginAccountFromUserAccountID(int userID)
        {
            VaultUserOperations userOperations = Vault.UserOperations;
            
            return userOperations.GetLoginAccountOfUser(userID);
        }
        

        public bool GroupExists(string name)
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
            ServerLoginAccountOperations loginAccountOps = MFilesServerApp.LoginAccountOperations;

            // Traitement   
            logins = loginAccountOps.GetLoginAccounts();

            // Sortie
            return logins;
        }

        UserAccounts getUserAccounts()
        {
            // Initialisation
            UserAccounts? users = null;
            VaultUserOperations userOps = Vault.UserOperations;

            // Traitement
            users = userOps.GetUserAccounts();

            // Sortie
            return users;
        }

        bool changeLicence(LoginAccount account, MFLicenseType licence)
        {
            // Initialisation
            bool success = true;
            ServerLoginAccountOperations loginAccountOps = MFilesServerApp.LoginAccountOperations;

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
            return MFilesServerApp.LoginAccountOperations.GetLoginAccount(account.AccountName);
        }

        /// <summary>
        /// Removes the license of a M-Files login account that is not in the maintained account list
        /// </summary>
        /// <param name="account">The login account whose license to remove</param>
        /// <returns>>True if operations was succeful (license was removed or account is protected), false otherwise</returns>
        public void DeleteAccountLicense(LoginAccount account)
        {
            ServerLoginAccountOperations loginOperations = MFilesServerApp.LoginAccountOperations;
            bool success = true;

            if (!Configuration.MaintainedAccounts.Contains(account.UserName))
            {
                try
                {
                    account.LicenseType = MFLicenseType.MFLicenseTypeNone;
                    loginOperations.ModifyLoginAccount(account);
                }
                catch (Exception e)
                {
                    success = false;
                }
            }
            
        }


        /// <summary>
        /// Removes the license of a M-Files login account that is not in the maintained account list
        /// </summary>
        /// <param name="accountName">The name of the account to remove</param>
        /// <returns>True if operations was succeful (license was removed or account is protected), false otherwise</returns>
        public void DeleteAccountLicense(string accountName)
        {
            LoginAccount account = MFilesServerApp.LoginAccountOperations.GetLoginAccount(accountName);
            DeleteAccountLicense(account);
        }

        public void ChangeAccountLicense(LoginAccount account, MFLicenseType licenseType)
        {
            ServerLoginAccountOperations loginOperations = MFilesServerApp.LoginAccountOperations;

            // TODO : Mettre en place un check pour le compte à ne PAS restaurer
            if (true)
            {
                account.LicenseType = licenseType;
                loginOperations.ModifyLoginAccount(account);
            }
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

        public bool AreValidCredentials(string username, string password)
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

        public UserPrincipal GetADUser(string username)
        {
            return UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, username);
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

        public List<LoginAccount> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public List<UserGroup> GetAllGroups(int serverId)
        {
            throw new NotImplementedException();
        }
    }
}
