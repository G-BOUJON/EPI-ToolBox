namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class MFilesCredential
    {
        public int Id { get; set; }
        public int ServerId {  get; set; }
        public string EncryptedUserName { get; set; }
        public string EncryptedPassword { get; set; }
        public string Domain { get; set; }
        


        public MFilesServer MFilesServer { get; set; }

        public MFilesCredential() { }
    }
}
