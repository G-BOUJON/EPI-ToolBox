namespace ToolBox_MVC.Services.DB
{
    public interface IGroupAccountRepository
    {
        Task SyncAccountGroupLink(int serverId, int mfilesGroupId, string accountName);
        Task SyncLinks(int serverId, int mfilesGroupId, HashSet<string> accountNames);
        Task SyncLinks(int serverId, int mfilesGroupId, HashSet<int> userIds);
    }
}
