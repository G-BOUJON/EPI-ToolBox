using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ToolBox_MVC.Models
{
    public class Config
    {
        public bool ActiveSuppression { get; set; }
        public bool ActiveRestauration { get; set; }
        public int? Frequence { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly Hour { get; set; }
        public List<Group> Groups { get; set; }
        public List<string> MaintainedAccounts { get; set; }
        public VaultCredentials VaultCredentials { get; set; }
        public ActiveDirectoryCredentials ActiveDirectoryCredentials { get; set; }
        public string ApiKey { get; set; }

        public Config(bool recursiveSearch, int delay, List<Group> groups)
        {
            this.ActiveSuppression = recursiveSearch;
            this.Frequence = delay;
            this.Groups = groups;
        }

        public Config(bool recursiveSearch, List<Group> groups)
        {
            this.ActiveSuppression = recursiveSearch;
            this.Groups = groups;
        }

        public Config(List<Group> groups)
        {
            this.Groups = groups;
        }

        public Config()
        {

        }

        public List<string> GetGroupsNames()
        {
            List<string> groupsNames = new List<string>();
            foreach (Group group in this.Groups)
            {
                groupsNames.Add("EPI\\" + group.name);
            }
            return groupsNames;
        }
    }
}
