using ToolBox_MVC.Models;
using ToolBox_MVC.Services.ActiveDirectory;

namespace ToolBox_MVC.Services.Factories
{
    public interface IADUsersHandlerFactory
    {
        IActiveDirectoryUsersHandler Create(ServerType server);
    }

    public class ActiveDirectoryUserHandlerFactory : IADUsersHandlerFactory
    {
        IConfigurationHandlerFactory _configFactory;
        public ActiveDirectoryUserHandlerFactory(IConfigurationHandlerFactory configFactory) 
        {
            _configFactory = configFactory;
        }

        public IActiveDirectoryUsersHandler Create(ServerType server)
        {
            ActiveDirectoryCredentials credentials = _configFactory.Create(server).GetConfiguration().ActiveDirectoryCredentials;
            return new ADUsersService(credentials);
        }
    }
}
