using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.Repository
{
    public interface IMFilesConnexionInfosService
    {
        Task<MFilesConnexionInfo> GetConnexionInfos(int serverId);
    }
}