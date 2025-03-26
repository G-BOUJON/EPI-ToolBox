using MFilesAPI;

namespace ToolBox.Models
{
    public class Account
    {
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public string Enabled { get; set; }
        public string FullName { get; set; }
        public string LicenseType { get; set; }
        public string ServerRoles { get; set; }
        public string UserName { get; set; }

        public Account()
        {
            AccountName = string.Empty;
            AccountType = string.Empty;
            DomainName = string.Empty;
            EmailAddress = string.Empty;
            Enabled = string.Empty;
            FullName = string.Empty;
            LicenseType = string.Empty;
            ServerRoles = string.Empty;
            UserName = string.Empty;
        }

        public Account(LoginAccount account)
        {
            AccountName = account.AccountName;
            AccountType = account.AccountType.ToString();
            DomainName = account.DomainName;
            EmailAddress = account.EmailAddress;
            Enabled = account.Enabled.ToString();
            FullName = account.FullName;
            LicenseType = account.LicenseType.ToString();
            ServerRoles = account.ServerRoles.ToString();
            UserName = account.UserName;
        }

        public string translateLicenseName()
        {
            List<string> enNames = new List<string> { "MFLicenseTypeNamedUserLicense", "MFLicenseTypeConcurrentLicense", "MFLicenseTypeReadOnlyLicense" };
            List<string> frNames = new List<string> { "Nominative", "Concurrente", "Lecture seule" };

            for (int i = 0; i < enNames.Count; i++)
            {
                if (enNames[i] == this.LicenseType)
                {
                    return frNames[i];
                }
            }

            return this.LicenseType;
        }
    }
}
