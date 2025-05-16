using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Services.DB
{
    public interface IGroupRepositoryold
    {
        Task SyncGroupsAsync(int serverId, List<MFilesGroup> groups);
        Task<List<MFilesGroup>> GetGroupsAsync(int serverId);
        Task<MFilesGroup> GetGroupAsync(int groupID);
        Task<MFilesGroup> GetGroupAsync(int serverID, string groupName);
        Task UpdateOrAddGroupAsync(MFilesGroup group);


    }
}
