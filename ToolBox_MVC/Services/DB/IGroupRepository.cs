using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services.DB
{
    public interface IGroupRepository
    {
        Task SyncGroupsAsync(int serverId, List<MFilesGroup> groups);
        Task<List<MFilesGroup>> GetGroupsAsync(int serverId);
    }
}
