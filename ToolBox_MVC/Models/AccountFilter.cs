using MFilesAPI;

namespace ToolBox.Models
{
    public enum MaintainedAccountType
    {
        Maintained,
        Unmaintained
    }
    public class AccountFilter
    {
        public List<MFLicenseType> LicenseTypes { get; set; }
        public List<MFLoginAccountType> AccountTypes { get; set; }
        public List<MaintainedAccountType> MaintainedTypes { get; set; }

        public AccountFilter() : this(
            new List<MFLicenseType>()
            {
                MFLicenseType.MFLicenseTypeNamedUserLicense,
                MFLicenseType.MFLicenseTypeConcurrentUserLicense,
                MFLicenseType.MFLicenseTypeReadOnlyLicense
            },
            new List<MFLoginAccountType>
            {
                MFLoginAccountType.MFLoginAccountTypeWindows,
                MFLoginAccountType.MFLoginAccountTypeMFiles
            },
            new List<MaintainedAccountType>
            {
                MaintainedAccountType.Maintained,
                MaintainedAccountType.Unmaintained
            }
        ){ }

        public AccountFilter(List<MFLicenseType> licenseTypes, List<MFLoginAccountType> accountTypes, List<MaintainedAccountType> maintainedTypes)
        {
            LicenseTypes = licenseTypes;
            AccountTypes = accountTypes;
            MaintainedTypes = maintainedTypes;
        }

        public List<LoginAccount> filterAccounts(List<LoginAccount> accounts, List<string> maintainedAccounts)
        {
            // Initialisation
            List<LoginAccount> filteredAccounts = new List<LoginAccount>();

            // Traitement
            foreach (LoginAccount account in accounts)
            {
                if (this.LicenseTypes.Contains(account.LicenseType) && this.AccountTypes.Contains(account.AccountType))
                {
                    if ((MaintainedTypes.Contains(MaintainedAccountType.Maintained) && maintainedAccounts.Contains(account.UserName))
                        || (MaintainedTypes.Contains(MaintainedAccountType.Unmaintained) && !maintainedAccounts.Contains(account.UserName)))
                    {
                        filteredAccounts.Add(account);
                    }
                }
            }
            // Sortie

            return filteredAccounts;
        }

        public int getNumberFilters()
        {
            int total = 0;

            total += this.LicenseTypes.Count();
            total += this.AccountTypes.Count();
            total += this.MaintainedTypes.Count();

            return total;
        }
    }
}
