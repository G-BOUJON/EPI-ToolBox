using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdService
    {
        bool IsUserActive(int adId, string userName);
        bool AreValidCredentials(int adId, string userName, string password);
        bool GroupExists(int adId, string groupName);
    }

    public class ActiveDirectoryService : IAdService
    {
        private readonly IAdConnectorFactory _connectorFactory;

        public ActiveDirectoryService(IAdConnectorFactory connectorFactory)
        {
            _connectorFactory = connectorFactory;
        }

        public bool AreValidCredentials(int adId, string userName, string password)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(adId))
            {
                return connector.ValidateCredentials(userName, password);
            }
        }

        public bool GroupExists(int adId, string groupName)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(adId))
            { 
                return GroupPrincipal.FindByIdentity(connector, groupName) != null;
            }
        }

        public bool IsUserActive(int adId, string userName)
        {
            using (var connector = _connectorFactory.CreatePrincipalContext(adId))
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
