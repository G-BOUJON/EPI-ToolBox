using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public class ADUsersService : IActiveDirectoryUsersHandler
    {
        private PrincipalContext PrincipalContext { get; set; }

        public ADUsersService(IConfigurationHandler configurationHandler) : this(configurationHandler.GetConfiguration().ActiveDirectoryCredentials)
        {
        }

        public ADUsersService(ActiveDirectoryCredentials credentials)
        {
            PrincipalContext = new PrincipalContext(
                contextType: ContextType.Domain,
                name: credentials.Domain,
                container: credentials.Container,
                userName: credentials.Username,
                password: credentials.Password
                );
        }

        private UserPrincipal GetUser(string username)
        {
            return UserPrincipal.FindByIdentity(PrincipalContext, IdentityType.SamAccountName, username);
        }

        public bool IsUserActive(string username)
        {
            UserPrincipal user = GetUser(username);
            if (user == null)
            {
                throw new ArgumentNullException("username", "Ce compte n'existe pas dans l'AD");
            }
            DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;

            if (de.NativeGuid == null)
            {
                return false;
            }

            if (de.Properties["userAccountControl"].Value == null)
            {
                return false;
            }

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        public bool AreValidCredentials(string username, string password)
        {
            return PrincipalContext.ValidateCredentials(username, password);
        }

        public bool GroupExists(string groupName)
        {
            return GroupPrincipal.FindByIdentity(PrincipalContext, groupName) != null;
        }
    }
}
