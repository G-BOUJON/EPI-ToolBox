namespace ToolBox_MVC.Models
{
    public class ADCredential
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public string Container { get; set; }
        public string EncryptedUsername { get; set; }
        public string EncryptedPassword { get; set; }
    }
}
