using MFilesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToolBox.Models;
using ToolBox.Services.LicenseManagerCert;

namespace ToolBox.Pages.LicenseManagerCert
{
    [Authorize]
    public class UsersListModel : PageModel
    {
        private readonly ILogger<UsersListModel> _logger;
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        public Config config { get; set; }
        public AccountFilter filter { get; set; }

        public UsersListModel(ILogger<UsersListModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            WebHostEnvironment = webHostEnvironment;
            this.filter = new AccountFilter();
        }

        public void OnGet()
        {
            JsonConfService confService = new JsonConfService();
            this.config = confService.getConf();
        }

        public void OnPostRefreshList()
        {
            JsonConfService cf = new JsonConfService();
            MFilesUsersService mf = new MFilesUsersService();
            JsonLoginAccountsService la = new();
            la.updateList(mf.getList(cf.getConf().groups));
        }

        public void OnPostMaintainAllSelectedLicenses()
        {
            List<Account> accounts = this.getAccounts();
            List<Account> accountsToDelete = new List<Account>();
            JsonConfService jsonConfService = new JsonConfService();

            foreach (Account account in accounts)
            {
                if (!string.IsNullOrEmpty(Request.Form[account.UserName]))
                {
                    jsonConfService.addMaintainedAccount(account.UserName);
                }
            }


            Console.WriteLine("Maintenance de " + accountsToDelete.Count + " comptes");
        }

        public void OnPostDeleteAllSelectedLicenses()
        {
            List<Account> accounts = this.getAccounts();
            List<Account> accountsToDelete = new List<Account>();
            MFilesUsersService mfUsersService = new MFilesUsersService();

            foreach (Account account in accounts)
            {
                if (!string.IsNullOrEmpty(Request.Form[account.UserName]))
                {
                    mfUsersService.deleteLicence(mfUsersService.convertToLoginAccount(account));
                    accountsToDelete.Add(account);
                }
            }
            
            Console.WriteLine("Suppression de " + accountsToDelete.Count + " comptes");
        }

        public void OnPostDeleteLicense()
        {
            MFilesUsersService mfUsersService = new MFilesUsersService();

            Account account = new Account();

            foreach (Account acnt in this.getAccounts())
            {
                if (acnt.UserName == Request.Form["deleteLicense"])
                {
                    account = acnt; break;
                }
            }
            JsonConfService cf = new JsonConfService();

            mfUsersService.deleteLicence(mfUsersService.convertToLoginAccount(account));
            Console.WriteLine("Licence retiree pour " + Request.Form["deleteLicense"]);
        }

        public void OnPostMaintain()
        {
            JsonConfService confService = new JsonConfService();
            confService.addMaintainedAccount(Request.Form["maintain"]);
        }

        public void OnPostUnMaintain()
        {
            JsonConfService confService = new JsonConfService();
            confService.deleteMaintainedAccount(Request.Form["unmaintain"]);
        }

        public void OnPostChangeFilter()
        {
            this.filter = new AccountFilter();

            if (Request.Form["nominative"] == "on")
            {
                this.filter.LicenseType.Add("MFLicenseTypeNamedUserLicense");
            }
            if (Request.Form["lectureSeule"] == "on")
            {
                this.filter.LicenseType.Add("MFLicenseTypeReadOnlyLicense");
            }
            if (Request.Form["concurrente"] == "on")
            {
                this.filter.LicenseType.Add("MFLicenseTypeConcurrentUserLicense");
            }
            if (Request.Form["windows"] == "on")
            {
                this.filter.AccountType.Add("MFLoginAccountTypeWindows");
            }
            if (Request.Form["mfiles"] == "on")
            {
                this.filter.AccountType.Add("MFLoginAccountTypeMFiles");
            }
            if (Request.Form["maintenue"] == "on")
            {
                this.filter.Maintained = true;
            }
            if (Request.Form["nonMaintenue"] == "on")
            {
                this.filter.UnMaintained = true;
            }
        }

        public List<Account> getAccounts()
        {
            JsonLoginAccountsService la = new JsonLoginAccountsService();
            JsonConfService confService = new JsonConfService();
            List<Account> nonExistingAccounts = la.getAccounts().ToList();
            this.config = confService.getConf();

            return this.filter.filterAccounts(nonExistingAccounts, this.config.maintainedAccounts);
        }

        public bool isMaintained(string username)
        {
            // Initialiation
            bool isMaintained = false;
            JsonConfService confService = new JsonConfService();
            this.config = confService.getConf();

            // Traitement
            foreach (string user in this.config.maintainedAccounts)
            {
                if (user == username) { isMaintained = true; break; }
            }

            // Sortie
            return isMaintained;
        }

        public bool isFiltered(string name)
        {
            bool isFiltered = false;

            foreach (var filter in this.filter.LicenseType)
            {
                if (filter == name)
                {
                    isFiltered = true;
                }
            }
            foreach (var filter in this.filter.AccountType)
            {
                if (filter == name)
                {
                    isFiltered = true;
                }
            }

            return isFiltered;
        }
    }
}
