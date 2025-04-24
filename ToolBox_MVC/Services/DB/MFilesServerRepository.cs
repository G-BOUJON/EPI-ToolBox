using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Data;

namespace ToolBox_MVC.Services.DB
{
    public interface IMfilesServerRepository
    {
        MFilesServer GetServerInfos(int serverId);
        MFilesServer GetServerInfos(string serverName);
        IEnumerable<MFilesServer> GetServers();
    }

    public class MFilesServerRepository : IMfilesServerRepository
    {
        private readonly ToolBoxDbContext _dbContext;

        public MFilesServerRepository(ToolBoxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public MFilesServer GetServerInfos(int serverId)
        {
            MFilesServer? server = _dbContext.MFilesServers.FirstOrDefault(s => s.Id == serverId);
            ArgumentNullException.ThrowIfNull(server);
            return server;
        }

        public MFilesServer GetServerInfos(string serverName)
        {
            var server = _dbContext.MFilesServers.FirstOrDefault(s => s.Name == serverName);
            ArgumentNullException.ThrowIfNull(server);
            return server;
        }

        public IEnumerable<MFilesServer> GetServers()
        {
            return _dbContext.MFilesServers;
        }
    }
}
