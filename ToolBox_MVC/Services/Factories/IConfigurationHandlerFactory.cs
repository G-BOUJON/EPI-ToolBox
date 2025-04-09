using ToolBox_MVC.Services.JsonServices;

namespace ToolBox_MVC.Services.Factories
{
    public interface IConfigurationHandlerFactory
    {
        IConfigurationHandler Create(ServerType serverType);
    }

    public class ConfigurationHandlerFactory : IConfigurationHandlerFactory
    {
        public ConfigurationHandlerFactory() { }

        public IConfigurationHandler Create(ServerType serverType)
        {
            return new JsonConfService(serverType);
        }
    }
}
