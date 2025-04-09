using ToolBox_MVC.Services.MFiles;

namespace ToolBox_MVC.Services.Factories
{
    public interface IMFilesUsersHandlerFactory
    {
        IMFilesUsersHandler Create(ServerType server);
    }

    public class MfilesUsersHandlerFactory : IMFilesUsersHandlerFactory
    {
        private readonly IConfigurationHandlerFactory _factory;
        public MfilesUsersHandlerFactory(IConfigurationHandlerFactory factory)
        {
            _factory = factory;
        }

        public IMFilesUsersHandler Create(ServerType server)
        {
            IConfigurationHandler configurationHandler = _factory.Create(server);
            return new MFilesUsersService(server,configurationHandler);
        }
    }
}
