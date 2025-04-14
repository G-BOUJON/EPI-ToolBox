using MFilesAPI;
using System.ComponentModel.DataAnnotations;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Models
{
    public interface IAccount
    {
        string AccountName { get; set; }
        MFLoginAccountType AccountType { get; set; }
        string DomainName { get; set; }
        string EmailAddress { get; set; }
        bool Enabled { get; set; }
        string FullName { get; set; }
        MFLicenseType LicenseType { get; set; }
        MFLoginServerRole ServerRoles { get; set; }
        string UserName { get; set; }
    }

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

            return TranslatorService.TranslateMFLicense(LicenseType);
           
           
        }

        public static List<Account> ConvertLoginAccountList(List<LoginAccount> loginAccounts)
        {
            List<Account> accounts = new List<Account>();

            foreach(LoginAccount loginAccount in loginAccounts)
            {
                accounts.Add(new Account(loginAccount));
            }

            return accounts;
        }
    }
}
