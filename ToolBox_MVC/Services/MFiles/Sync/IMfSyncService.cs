namespace ToolBox_MVC.Services.MFiles.Sync
{
    public interface IMfSyncService
    {
        Task SyncAccountsAsync(int serverId);
        Task SyncGroupsAsync(int serverId);
        bool TryConnections(int serverId);
    }
}
