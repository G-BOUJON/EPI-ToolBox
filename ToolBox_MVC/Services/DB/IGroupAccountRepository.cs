namespace ToolBox_MVC.Services.DB
{
    public interface IGroupAccountRepository
    {
        Task SyncAccountGroupLink(int serverId, int mfilesGroupId, string accountName);
        Task SyncLinks(int serverId, int mfilesGroupId, List<string> accountNames);
    }
}
