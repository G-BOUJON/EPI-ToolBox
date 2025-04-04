using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ToolBox_MVC.Models
{
    public class ActiveDirectoryCredentials
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Domain { get; set; }
        [Required]
        public string Container { get; set; }
    }
}
