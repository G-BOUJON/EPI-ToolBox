using MFilesAPI;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public abstract class AccountList
    {
        protected readonly IMFilesUsersHandler _usersHandler;
        protected readonly IAccountsListHandler _accountsListHandler;
        
        public Config Configuration { get; set; }
        public AccountFilter Filter { get; set; }
        public ServerType Server { get; set; }

        public AccountList(ServerType server, IMFilesUsersHandlerFactory mfilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory) : this(server, new AccountFilter(), mfilesFactory, accountsListHandlerFactory) { }

        public AccountList(ServerType server, AccountFilter filter, IMFilesUsersHandlerFactory mfilesFactory, IAccountsListHandlerFactory accountsListHandlerFactory)
        {
            Server = server;
            _usersHandler = mfilesFactory.Create(server);
            _accountsListHandler = accountsListHandlerFactory.Create(server);
            this.Filter = filter;
            Configuration = _usersHandler.Configuration;
            

        }

        public abstract List<Account> GetAccounts();
        public abstract void UpdateList();
        

        public bool FilterContains(MFLicenseType licenseType)
        {
            return Filter.LicenseTypes.Contains(licenseType);
        }

        public bool FilterContains(MFLoginAccountType accountType)
        {
            return Filter.AccountTypes.Contains(accountType);
        }

        public bool FilterContains(MaintainedAccountType maintainedAccountType)
        {
            return Filter.MaintainedTypes.Contains(maintainedAccountType);
        }


        /// <summary>
        /// Update both restoration and suppression lists from M-Files
        /// </summary>
        protected void UpdateAccounts()
        {
            Accounts mfAccounts = new Accounts(Account.ConvertLoginAccountList(_usersHandler.GetSuppressionList()), Account.ConvertLoginAccountList(_usersHandler.GetRestorationList()));
            _accountsListHandler.UpdateAccounts(mfAccounts);
        }
    }
}
