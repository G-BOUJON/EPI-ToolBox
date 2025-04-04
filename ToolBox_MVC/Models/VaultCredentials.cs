using System.ComponentModel.DataAnnotations;

namespace ToolBox_MVC.Models
{
    public class VaultCredentials
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Domain { get; set; }
        [Required]
        public string ProtocolSequence { get; set; }
        [Required]
        public string NetworkAddress { get; set; }
        [Required]
        public string EndPoint { get; set; }
        [Required]
        public string Guid { get; set; }
    }
}
