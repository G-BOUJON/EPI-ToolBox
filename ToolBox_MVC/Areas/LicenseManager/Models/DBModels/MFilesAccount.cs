using MFilesAPI;
using System.ComponentModel.DataAnnotations;

namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class MFilesAccount
    {
        
        public int Id { get; set; }
        public string AccountName {  get; set; }
        public string Domain {  get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int AccountType { get; set; }
        public int License {  get; set; }
        public int ServerRole { get; set; }
        public bool Enabled { get; set; }
        public bool Maintained { get; set; }
        public bool Active { get; set; }

        
        public int ServerId { get; set; }
        

        public virtual ICollection<MFilesGroup> Groups { get; set; }

        public MFilesAccount() { }

        public MFilesAccount(LoginAccount logAccount, int serverID)
        {
            ServerId = serverID;
            AccountName = logAccount.AccountName;
            Domain = logAccount.DomainName;
            EmailAddress = logAccount.EmailAddress;
            FullName = logAccount.FullName;
            UserName = logAccount.UserName;
            Enabled = logAccount.Enabled;
            ServerRole = (int)logAccount.ServerRoles;
            License = (int)logAccount.LicenseType;
            AccountType = (int)logAccount.AccountType;
            Maintained = false;
            Active = false;
            UserId = 0;
        }
        
    }
}
