using System.ComponentModel.DataAnnotations;

namespace ToolBox_MVC.Models
{
    public class MFilesConnexionInfo
    {
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Domain { get; set; }
        
        public string ProtocolSequence { get; set; }
        
        public string NetworkAddress { get; set; }
        
        public string EndPoint { get; set; }
        
        public string VaultGuid { get; set; }

        public MFilesConnexionInfo() { }
    }
}
