using MFilesAPI;
using ToolBox_MVC.Services;

namespace ToolBox.Models
{
    public class Account
    {
        public string AccountName { get; set; }
        public MFLoginAccountType AccountType { get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public bool Enabled { get; set; }
        public string FullName { get; set; }
        public MFLicenseType LicenseType { get; set; }
        public MFLoginServerRole ServerRoles { get; set; }
        public string UserName { get; set; }

        public Account() { }

        public Account(LoginAccount account)
        {
            AccountName = account.AccountName;
            AccountType = account.AccountType;
            DomainName = account.DomainName;
            EmailAddress = account.EmailAddress;
            Enabled = account.Enabled;
            FullName = account.FullName;
            LicenseType = account.LicenseType;
            ServerRoles = account.ServerRoles;
            UserName = account.UserName;
        }

        

        public string TranslateLicenseType()
        {

            return TranslatorService.TranslateMFLicense(this.LicenseType);
           
           
        }
    }
}
