namespace ToolBox_MVC.Models
{
    public class ADAccount
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public string Name { get; set; }
        public string EmailAdress { get; set; } 
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public ActiveDirectory ActiveDirectory { get; set; }
        public ICollection<ADGroup> Groups { get; set; }

        public ADAccount()
        {

        }

        public ADAccount(string guid, string name, string email, string displayName, bool enabled = false)
        {
            GUID = guid;
            Name = name;
            
            if (email == null)
            {
                EmailAdress = string.Empty;
            }
            else
            {
                EmailAdress = email;
            }

            if (DisplayName == null)
            {
                DisplayName = string.Empty;
            }
            else
            {
                DisplayName = displayName;
            }

            Enabled = enabled;
        }



    }
}
