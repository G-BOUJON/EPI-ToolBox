/* Modified by GBOUJON - Date : 2025-03-27 */

using MFilesAPI;

namespace ToolBox_MVC.Models
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

        public AccountFilter() 
        {
            LicenseTypes = new List<MFLicenseType>()
            {
                MFLicenseType.MFLicenseTypeNamedUserLicense,
                MFLicenseType.MFLicenseTypeConcurrentUserLicense,
                MFLicenseType.MFLicenseTypeReadOnlyLicense
            };
            AccountTypes = new List<MFLoginAccountType>
            {
                MFLoginAccountType.MFLoginAccountTypeWindows,
                MFLoginAccountType.MFLoginAccountTypeMFiles
            };
            MaintainedTypes = new List<MaintainedAccountType>
            {
                MaintainedAccountType.Maintained,
                MaintainedAccountType.Unmaintained
            };
        }

        public AccountFilter(List<MFLicenseType> licenseTypes, List<MFLoginAccountType> accountTypes, List<MaintainedAccountType> maintainedTypes)
        {
            LicenseTypes = licenseTypes;
            AccountTypes = accountTypes;
            MaintainedTypes = maintainedTypes;
        }

        public List<Account> filterAccounts(List<Account> accounts, List<string> maintainedAccounts)
        {
            // Initialisation
            List<Account> filteredAccounts = new List<Account>();

            // Traitement
            foreach (Account account in accounts)
            {
                if (LicenseTypes.Contains(account.LicenseType) && AccountTypes.Contains(account.AccountType))
                {
                    if (MaintainedTypes.Contains(MaintainedAccountType.Maintained) && maintainedAccounts.Contains(account.UserName)
                        || MaintainedTypes.Contains(MaintainedAccountType.Unmaintained) && !maintainedAccounts.Contains(account.UserName))
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

            total += LicenseTypes.Count();
            total += AccountTypes.Count();
            total += MaintainedTypes.Count();

            return total;
        }
    }
}
