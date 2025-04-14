using ToolBox_MVC.Areas.LicenseManager.Data;
using ToolBox_MVC.Areas.LicenseManager.Services.DBServices;
using ToolBox_MVC.Services.JsonServices;

namespace ToolBox_MVC.Services.Factories
{
    public interface IAccountsListHandlerFactory
    {
        IAccountsListHandler Create(ServerType server);
    }

    

    public class DbAccountsServiceFactory : IAccountsListHandlerFactory
    {

        private readonly LicenseManagerDBContext _dbContext;
        public DbAccountsServiceFactory(LicenseManagerDBContext context)
        {
            _dbContext = context;
        }

        public IAccountsListHandler Create(ServerType server)
        {
            return new DbAccountsService(server, _dbContext);
        }
    }
}
