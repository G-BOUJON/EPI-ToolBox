namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class MFilesGroup
    {
        public int Id { get; set; }
        public int MFilesId { get; set; }
        public int? ServerId { get; set; }
        public string Name { get; set; }

        public ICollection<MFilesAccount> Accounts { get; set; }
        public MFilesServer Server { get; set; }
    }
}
