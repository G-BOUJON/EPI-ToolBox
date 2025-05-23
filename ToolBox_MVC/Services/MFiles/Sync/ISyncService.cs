﻿namespace ToolBox_MVC.Services.MFiles.Sync
{
    public interface ISyncService
    {
        Task SyncAccountsAsync(int serverId);
        Task SyncGroupsAsync(int serverId);
        Task SyncGroupsAccountsLinksAsync(int serverId);
        Task SyncUserAccountAsync(int serverId);
    }
}
