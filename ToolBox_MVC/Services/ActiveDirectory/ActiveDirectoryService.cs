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
        bool IsMemberOf(int serverID, string userName, string groupName);
        List<GroupPrincipal> GetGroupPrincipals(int adID);
        List<UserPrincipal> GetUserPrincipals(int adID);
    }

    public class ActiveDirectoryService : IAdService
    {
        private readonly IAdConnectorFactory _connectorFactory;

        private int _serverID;
        private PrincipalContext _context;

        public ActiveDirectoryService(IAdConnectorFactory connectorFactory)
        {
            _connectorFactory = connectorFactory;
        }

        public List<UserPrincipal> GetUserPrincipals(int adID)
        {
            CreateConnection(adID);
            var connector = _context;

            List<UserPrincipal> userPrincipals = new List<UserPrincipal>();

            UserPrincipal u = new UserPrincipal(connector);
            PrincipalSearcher search = new PrincipalSearcher(u);

            foreach (UserPrincipal user in search.FindAll())
            {
                if (user != null)
                {
                    userPrincipals.Add(user);
                }
            }

            return userPrincipals;
        }

        public List<GroupPrincipal> GetGroupPrincipals(int adID)
        {
            CreateConnection(adID);
            var connector = _context;

            List<GroupPrincipal> groupPrincipals = new List<GroupPrincipal>();

            GroupPrincipal g = new GroupPrincipal(connector);
            PrincipalSearcher search = new PrincipalSearcher(g);

            foreach (GroupPrincipal group in search.FindAll())
            {
                if (group != null)
                {
                    groupPrincipals.Add(group);
                }
            }
            return groupPrincipals;
        }

        public bool AreValidCredentials(int serverID, string userName, string password)
        {
            CreateConnection(serverID);
            var connector = _context;
            
            return connector.ValidateCredentials(userName, password);
            
        }

        public bool GroupExists(int serverID, string groupName)
        {
            CreateConnection(serverID);
            var connector = _context;
            
            return GroupPrincipal.FindByIdentity(connector, groupName) != null;

        }

        public bool IsMemberOf(int serverID, string userName, string groupName)
        {
            CreateConnection(serverID);
            var connector = _context;
        
            var user = UserPrincipal.FindByIdentity(connector, userName);
            var group = GroupPrincipal.FindByIdentity(connector, groupName);

            if (user == null || group == null)
            {
                throw new ArgumentException();
            }

            return user.IsMemberOf(group);
        
        }

        public bool IsUserActive(int serverID, string userName)
        {
            CreateConnection(serverID);
            var connector = _context;
        
            using (var user = UserPrincipal.FindByIdentity(connector, userName))
            {

                if (user == null)
                {
                    throw new ArgumentException("User doesn't exist on this server");
                }

                if (user.Name != userName)
                {
                    return false;
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

        private void CreateConnection(int serverID)
        {
            if (serverID != _serverID)
            {
                _serverID = serverID;
                _context = _connectorFactory.CreatePrincipalContext(_serverID);
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
