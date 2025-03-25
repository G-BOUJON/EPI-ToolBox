using MFilesAPI;
using Microsoft.AspNetCore.Authentication;
using System.DirectoryServices.AccountManagement;
using ToolBox.Models;

namespace ToolBox.Services.LicenseManagerCert
{
    public class MFilesUsersService
    {
        public MFilesServerApplication mfServerApplication { get; set; }
        public Vault vault { get; set; }
        public PrincipalContext pc { get; set; }
        public MFilesUsersService()
        {
            JsonConfService jsonConfService = new JsonConfService();
            Config config = jsonConfService.getConf();

            mfServerApplication = new MFilesServerApplication();
            mfServerApplication.Connect(
                AuthType: MFAuthType.MFAuthTypeSpecificWindowsUser,
                UserName: config.vaultCredentials.username,
                Password: config.vaultCredentials.password,
                Domain: config.vaultCredentials.domain,
                ProtocolSequence: config.vaultCredentials.protocolSequence,
                NetworkAddress: config.vaultCredentials.networkAddress,
                Endpoint: config.vaultCredentials.endPoint);
            vault = mfServerApplication.LogInToVault(config.vaultCredentials.guid);

            pc = new(contextType: ContextType.Domain, name: config.activeDirectoryCredentials.domain, container: config.activeDirectoryCredentials.container, userName: config.activeDirectoryCredentials.username, password: config.activeDirectoryCredentials.password);
        }

        public List<LoginAccount> getList(List<Group> groupNames)
        {
            // Initialisation
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
            // Initialisation
            LoginAccounts accounts = getLoginAccounts();
            LoginAccount convertedAccount = new LoginAccount();

            // Traitement
            foreach (LoginAccount lgAccount in accounts)
            {
                if (lgAccount.UserName == account.UserName)
                {
                    convertedAccount = lgAccount; break;
                }
            }

            // Sortie
            return convertedAccount;
        }

        public bool deleteLicence(LoginAccount account)
        {
            // Initialisation
            bool success = true;
            JsonHistoryService historyService = new JsonHistoryService();
            JsonConfService jsonConfService = new JsonConfService();
            ServerLoginAccountOperations loginAccountOps = mfServerApplication.LoginAccountOperations;
            bool maintainedAccount = false;

            foreach (string acnt in jsonConfService.getConf().maintainedAccounts)
            {
                if (acnt.Equals(account.UserName))
                {
                    maintainedAccount = true;
                    Console.WriteLine("La licence pour ce compte est maintenue");
                }
            }

            // Traitement
            try
            {
                if (!maintainedAccount)
                {
                    historyService.addDeletedAccount(new Account(account));
                    account.LicenseType = MFLicenseType.MFLicenseTypeNone;
                    loginAccountOps.ModifyLoginAccount(account);
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            // Sortie
            return success;
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
