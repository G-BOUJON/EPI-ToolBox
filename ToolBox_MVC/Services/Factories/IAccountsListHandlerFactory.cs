using ToolBox_MVC.Services.JsonServices;

namespace ToolBox_MVC.Services.Factories
{
    public interface IAccountsListHandlerFactory
    {
        IAccountsListHandler Create(ServerType server);
    }

    public class AccountsListHandlerFactory : IAccountsListHandlerFactory
    {

        public AccountsListHandlerFactory() { }

        public IAccountsListHandler Create(ServerType server)
        {
            return new JsonLoginAccountsService(server);
        }

    }
}
