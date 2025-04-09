using ToolBox_MVC.Services.JsonServices;

namespace ToolBox_MVC.Services.Factories
{
    public interface IAccountsHistoryHandlerFactory
    {
        IAccountsHistoryHandler Create(ServerType id);
    }

    public class AccountsHistoryHandlerFactory : IAccountsHistoryHandlerFactory
    {
        private readonly IMFilesUsersHandlerFactory _filesUsersHandlerFactory;
        public AccountsHistoryHandlerFactory(IMFilesUsersHandlerFactory filesUsersHandlerFactory)
        {
            _filesUsersHandlerFactory = filesUsersHandlerFactory;
        }

        public IAccountsHistoryHandler Create(ServerType id)
        {
            IMFilesUsersHandler mFilesUsersHandler = _filesUsersHandlerFactory.Create(id);
            return new JsonHistoryService(id, mFilesUsersHandler);
        }
    }
}
