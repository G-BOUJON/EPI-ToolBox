
namespace ToolBox_MVC.Services.ActiveDirectory.Sync
{
    public interface IActiveDirectorySyncService
    {
        Task SyncActiveDirectory(int activeDirectoryID);
        Task SyncADAccountAsync(int adID);
        Task SyncADGroupAsync(int adID);
    }
}