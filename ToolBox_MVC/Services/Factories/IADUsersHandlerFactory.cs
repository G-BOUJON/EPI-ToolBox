using ToolBox_MVC.Models;
using ToolBox_MVC.Services.ActiveDirectory;

namespace ToolBox_MVC.Services.Factories
{
    public interface IADUsersHandlerFactory
    {
        IActiveDirectoryUsersHandler Create(ActiveDirectoryCredentials credentials);
    }

    public class ActiveDirectoryUserHandlerFactory : IADUsersHandlerFactory
    {
        public ActiveDirectoryUserHandlerFactory() { }

        public IActiveDirectoryUsersHandler Create(ActiveDirectoryCredentials crendentials)
        {
            return new ADUsersService(crendentials);
        }
    }
}
