namespace ToolBox_MVC.Models
{
    public class ActiveDirectory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ContextType { get; set; }
        public string Container { get; set; }
        public Credentials EncryptedCredentials { get; set; }
        public DateOnly LastSync { get; set; }

        public ActiveDirectory() { }

        public ActiveDirectory(string name, int contextType, string container, string username, string password) : this(name, contextType, container, new Credentials(username, password)) { }
        public ActiveDirectory(string name, int contextType, string container, Credentials encryptedCredentials)
        {
            Name = name;
            ContextType = contextType;
            Container = container;
            EncryptedCredentials = encryptedCredentials;
        }

        public ActiveDirectory Clone()
        {
            ActiveDirectory ad = new ActiveDirectory();
            ad.ID = ID;
            ad.Name = Name;
            ad.ContextType = ContextType;
            ad.Container = Container;
            ad.EncryptedCredentials = new Credentials(EncryptedCredentials.Username, EncryptedCredentials.Password);
            return ad;
        }
        


    }
}
