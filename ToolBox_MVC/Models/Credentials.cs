using System.Diagnostics.CodeAnalysis;

namespace ToolBox_MVC.Models
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain {  get; set; }

        
        public Credentials(string username, string password, string domain)
        {
            Username = username;
            Password = password;
            Domain = domain;
        }
    }
}
