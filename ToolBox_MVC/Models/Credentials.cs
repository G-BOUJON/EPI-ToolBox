using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ToolBox_MVC.Models
{
    public class Credentials
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Domain {  get; set; }


        public Credentials()
        {

        }
        
        public Credentials(string username, string password, string domain)
        {
            Username = username;
            Password = password;
            Domain = domain;
        }
    }
}
