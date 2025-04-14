using MFilesAPI;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using ToolBox_MVC.Services;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class DBAccount : IAccount
    {
        [Required]
        [Key]
        public string AccountName { get; set; }
        [Key]
        public ServerType Server {  get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public bool Enabled { get; set; }
        public string FullName  { get; set; }
        public bool MaintainedID { get; set; }
        public string UserName { get; set; }
        public MFLicenseType LicenseType { get; set; }
        public MFLoginAccountType AccountType { get; set; }
        public MFLoginServerRole ServerRoles { get; set; }

        public DBAccount() { }

        public DBAccount(LoginAccount account, ServerType server) : this(account, server, false) { } 
        public DBAccount(LoginAccount account, ServerType server, bool maintained)
        {
            this.AccountName = account.AccountName;
            this.DomainName = account.DomainName;
            this.EmailAddress = account.EmailAddress;
            this.Enabled = account.Enabled;
            this.FullName = account.FullName;
            this.LicenseType = account.LicenseType;
            this.AccountType = account.AccountType;
            this.ServerRoles = account.ServerRoles;
            this.UserName = account.UserName;

            this.Server = server;

            this.MaintainedID = maintained;
        }

        public Account GetAccount()
        {
            Account account = new Account();

            account.AccountName = AccountName;
            account.DomainName = DomainName;
            account.EmailAddress = EmailAddress;
            account.FullName = FullName;
            account.Enabled = Enabled;
            account.AccountType = AccountType;
            account.LicenseType = LicenseType;
            account.ServerRoles = ServerRoles;
            account.UserName = UserName;

            return account;
        }

        public bool Equals(LoginAccount account)
        {
            return
                account.AccountName == AccountName &&
                account.DomainName == DomainName &&
                account.EmailAddress == EmailAddress &&
                account.FullName == FullName &&
                account.Enabled == Enabled &&
                account.AccountType == this.AccountType &&
                account.LicenseType == this.LicenseType &&
                account.ServerRoles == this.ServerRoles &&
                account.UserName == UserName;
        }

        public void Update(LoginAccount account)
        {
            this.DomainName = account.DomainName;
            this.EmailAddress = account.EmailAddress;
            this.Enabled = account.Enabled;
            this.FullName = account.FullName;
            this.LicenseType = account.LicenseType;
            this.AccountType = account.AccountType;
            this.ServerRoles = account.ServerRoles;
        }

        public void Update(DBAccount account)
        {
            this.DomainName = account.DomainName;
            this.EmailAddress = account.EmailAddress;
            this.Enabled = account.Enabled;
            this.FullName = account.FullName;
            this.LicenseType = account.LicenseType;
            this.AccountType = account.AccountType;
            this.ServerRoles = account.ServerRoles;
        }

    }
}
