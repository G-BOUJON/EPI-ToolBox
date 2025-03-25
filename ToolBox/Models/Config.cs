using System.Text.RegularExpressions;

namespace ToolBox.Models
{
    public class Config
    {
        public bool active { get; set; }
        public int? frequence { get; set; }
        public string hour { get; set; }
        public List<Group> groups { get; set; }
        public List<string> maintainedAccounts { get; set; }
        public VaultCredentials vaultCredentials { get; set; }
        public ActiveDirectoryCredentials activeDirectoryCredentials { get; set; }
        public string apiKey { get; set; }

        public Config(bool recursiveSearch, int delay, List<Group> groups)
        {
            this.active = recursiveSearch;
            this.frequence = delay;
            this.groups = groups;
        }

        public Config(bool recursiveSearch, List<Group> groups)
        {
            this.active = recursiveSearch;
            this.groups = groups;
        }

        public Config(List<Group> groups)
        {
            this.groups = groups;
        }

        public Config()
        {

        }
    }
}
