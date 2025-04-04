using System.ComponentModel.DataAnnotations;

namespace ToolBox.Models
{
    public class ActiveDirectoryCredentials
    {
        [Required]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        public string domain { get; set; }
        [Required]
        public string container { get; set; }
    }
}
