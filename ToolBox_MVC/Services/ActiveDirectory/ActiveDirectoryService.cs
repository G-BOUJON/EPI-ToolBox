using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdService
    {
        bool IsUserActive(int serverID, string userName);
        bool AreValidCredentials(int serverID, string userName, string password);
        bool GroupExists(int serverID, string groupName);
    }

    public class ActiveDirectoryService : IAdService
    {
        private readonly IAdConnectorFactory _connectorFactory;

        public ActiveDirectoryService(IAdConnectorFactory connectorFactory)
        {
            _connectorFactory = connectorFactory;
        }

        public bool AreValidCredentials(int serverID, string userName, string password)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(serverID))
            {
                return connector.ValidateCredentials(userName, password);
            }
        }

        public bool GroupExists(int serverID, string groupName)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(serverID))
            { 
                return GroupPrincipal.FindByIdentity(connector, groupName) != null;
            }
        }

        public bool IsUserActive(int serverID, string userName)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(serverID))
            {
                using (var user = UserPrincipal.FindByIdentity(connector, userName))
                {

                    if (user == null)
                    {
                        throw new ArgumentNullException();
                    }

                    using (DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry)
                    {

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
                }
            }
        }
    }
}
