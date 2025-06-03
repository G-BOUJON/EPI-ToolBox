namespace ToolBox_MVC.Models
{
    public class ADGroup
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public string Name { get; set; }
        public ActiveDirectory ActiveDirectory { get; set; }
        public ICollection<ADAccount> Accounts { get; set; }

        public ADGroup() { }
        public ADGroup(string guid, string name) 
        {
            GUID = guid;
            Name = name;
        }
    }
}
