using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services.Repository
{
    public interface IMFilesServerRepository
    {
        MFilesServer GetMFilesServer(int serverId);
        List<MFilesServer> GetAllServers();
        void UpdateServer(MFilesServer server);
    }
}
