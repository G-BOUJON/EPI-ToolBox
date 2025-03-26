namespace ToolBox.Models
{
    public class AccountFilter
    {
        public List<string> LicenseType { get; set; }
        public List<string> AccountType { get; set; }
        public bool Maintained { get; set; }
        public bool UnMaintained { get; set; }

        public AccountFilter()
        {
            LicenseType = new List<string>();
            AccountType = new List<string>();
            UnMaintained = false;
            Maintained = false;
        }

        public List<Account> filterAccounts(List<Account> accounts, List<string> maintainedAccounts)
        {
            // Initialisation
            List<Account> filteredAccounts = new List<Account>();
            bool addAccount = true;
            bool satistfiesFilter = false;

            // Traitement 
            if (this.LicenseType.Count == 0 && this.AccountType.Count == 0 && this.Maintained && this.UnMaintained)
            {
                return accounts;
            }

            foreach (Account account in accounts)
            {
                foreach (string licenseType in this.LicenseType)
                {
                    if (account.LicenseType == licenseType)
                    {
                        satistfiesFilter = true;
                    }
                }
                if (!(satistfiesFilter && addAccount) && this.LicenseType.Count > 0)
                {
                    addAccount = false;
                }
                satistfiesFilter = false;

                foreach (string accountType in this.AccountType)
                {
                    if (account.AccountType == accountType)
                    {
                        satistfiesFilter = true;
                    }
                }
                if (!(satistfiesFilter && addAccount) && this.AccountType.Count > 0)
                {
                    addAccount = false;
                }
                satistfiesFilter = false;

                if ((this.Maintained && this.UnMaintained) || (!this.Maintained && !this.UnMaintained))
                {
                    satistfiesFilter = true;
                }
                if (this.Maintained && !this.UnMaintained)
                {
                    foreach (string maintainedAccount in maintainedAccounts)
                    {
                        if (maintainedAccount == account.UserName)
                        {
                            satistfiesFilter = true; break;
                        }
                    }
                }
                if (this.UnMaintained && !this.Maintained)
                {
                    satistfiesFilter = true;
                    foreach (string maintainedAccount in maintainedAccounts)
                    {
                        if (maintainedAccount == account.UserName)
                        {
                            satistfiesFilter = false; break;
                        }
                    }
                }
                if (!satistfiesFilter)
                {
                    addAccount = false;
                }
                satistfiesFilter = false;

                if (addAccount)
                {
                    filteredAccounts.Add(account);
                }

                addAccount = true;
            }

            // Sortie

            return filteredAccounts;
        }

        public int getNumberFilters()
        {
            int total = 0;

            total += this.LicenseType.Count();
            total += this.AccountType.Count();
            if (this.Maintained)
            {
                total++;
            }
            if (this.UnMaintained)
            {
                total++;
            }

            return total;
        }
    }
}
