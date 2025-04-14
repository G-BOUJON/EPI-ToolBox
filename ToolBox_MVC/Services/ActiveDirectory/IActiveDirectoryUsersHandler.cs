namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IActiveDirectoryUsersHandler
    {
        bool IsUserActive(string username);

        bool AreValidCredentials(string username, string password);

        bool GroupExists(string groupName);
    }
}
