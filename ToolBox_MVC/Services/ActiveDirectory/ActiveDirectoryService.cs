using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Authentication;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdService
    {
        bool IsUserActive(int serverID, string userName);
        bool AreValidCredentials(int serverID, string userName, string password);
        bool GroupExists(int serverID, string groupName);
        ADConnectionResult TryConnection(int serverId);
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
                        throw new ArgumentException("User doesn't exist on this server");
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

        public ADConnectionResult TryConnection(int serverId)
        {
            try
            {
                using var connector =_connectorFactory.CreatePrincipalContext(serverId);
                return ADConnectionResult.Success;
                
            }
            catch (PrincipalServerDownException)
            {
                return ADConnectionResult.Failed;
            }
            catch (AuthenticationException)
            {
                return ADConnectionResult.InvalidCredentials;
            }
        }
    }

    public enum ADConnectionResult
    {
        Success,
        InvalidCredentials,
        Failed
    }
}
