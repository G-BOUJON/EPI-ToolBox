using MFilesAPI;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ToolBox_MVC.Areas.LicenseManager.Data;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services;

namespace ToolBox_MVC.Areas.LicenseManager.Services.DBServices
{
    public class DbAccountsService : IAccountsListHandler
    {
        private readonly LicenseManagerDBContext _context;
        public ServerType Server { get; set; }
        
        public DbAccountsService(ServerType server, LicenseManagerDBContext context)
        {
            _context = context;
            Server = server;
        }

        
        private List<IAccount> ConvertToIAccountList(List<DBAccount> dbaccounts)
        {
            List<IAccount> accounts = new List<IAccount>();
            accounts.AddRange(dbaccounts);
            return accounts;
        }

        public async void UpdateAllAccounts(List<LoginAccount> allAccounts)
        {

            UpdateAllAccounts(ConvertToAccountList(allAccounts));
        }

        private async void UpdateAllAccounts(List<DBAccount> allaccounts)
        {
            List<Task> crudTasks = new List<Task>() {
                ModifyAccounts(allaccounts)
            };

            await Task.WhenAll(crudTasks);
        }

        public List<IAccount> GetDeletedAccounts()
        {
            return ConvertToIAccountList(GetDeletedDBAccounts());
        }

        public List<IAccount> GetRestoredAccounts()
        {
            return ConvertToIAccountList(GetRestoredDBAccount());
        }

        public void UpdateDeleteList(List<LoginAccount> suppresedAccount)
        {
            List<DBAccount> allAccount = ConvertToAccountList(suppresedAccount);
            List<DBAccount> resDB = GetRestoredDBAccount();
            allAccount.AddRange(resDB);
            UpdateAccounts(allAccount);
        }

        public void UpdateRestoreList(List<LoginAccount> restoredAccounts)
        {
            List<DBAccount> restoredDB = ConvertToAccountList(restoredAccounts);
            List<DBAccount> allAccount = GetDeletedDBAccounts();
            allAccount.AddRange(restoredDB);
            UpdateAccounts(allAccount);
        }


        private List<DBAccount> ConvertToAccountList(List<LoginAccount> lAccounts)
        {
            var list = new List<DBAccount>();
            foreach (LoginAccount account in lAccounts)
            {
                list.Add(new DBAccount(account,Server));
            }
            return list;

        }

        private IQueryable<DBAccount> GetDBAccounts()
        {
            var accounts = from dbaccount in _context.Accounts
                           select dbaccount;

            accounts = accounts.Where(dbaccount => dbaccount.Server == Server);


            return accounts;
        }

        public List<DBAccount> GetDeletedDBAccounts()
        {
            List<DBAccount> accounts = GetDBAccounts().ToList();

            accounts = accounts.Where(dbaccount => !dbaccount.LicenseType.Equals(MFLicenseType.MFLicenseTypeNone)).ToList();

            return accounts;
        }

        public List<DBAccount> GetRestoredDBAccount()
        {
            List<DBAccount> accounts = GetDBAccounts().ToList();

            accounts = accounts.Where(dbaccount => dbaccount.LicenseType.Equals(MFLicenseType.MFLicenseTypeNone)).ToList();

            return accounts.ToList();
        }

        private void DeleteDbAccounts(List<DBAccount> accounts)
        {
            List<DBAccount> dbAccounts = GetDBAccounts().ToList();

            dbAccounts = dbAccounts.Where(dbAccount => !accounts.Any(a => a.AccountName == dbAccount.AccountName)).ToList();

            foreach (DBAccount account in dbAccounts)
            {
                _context.Remove(account);
            }
        }

        private List<DBAccount> GetUpdateList(List<DBAccount> accounts)
        {
            List<DBAccount> dbAccounts = GetDBAccounts().ToList();

            dbAccounts = dbAccounts.Where(d => accounts.Any(a => a.AccountName == d.AccountName)).ToList();
            accounts = accounts.Where(a => !dbAccounts.Any(d => d.Equals(a))).ToList();

            return accounts;
        }

        private void UpdateAccounts(List<DBAccount> accoutsToUpdate)
        {
            foreach (var account in accoutsToUpdate)
            {
                try
                {
                    _context.Update<DBAccount>(GetUpdatedAccount(account));
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.ParamName + " - " + ex.Message);
                }
            }
        }

        private DBAccount GetUpdatedAccount(DBAccount account)
        {
            List<DBAccount> dbAccounts = GetDBAccounts().ToList();

            if (!dbAccounts.Any(d => d.AccountName == account.AccountName))
            {
                throw new ArgumentNullException(account.AccountName, "This account doesn't exists in the DB");
            }
            
            DBAccount dBAccount = dbAccounts.Find(d => d.AccountName == account.AccountName);
            dBAccount.Update(account);

            return dBAccount;
        }

        private List<DBAccount> GetAccountsToAdd(List<DBAccount> accounts)
        {
            var dbAccounts = GetDBAccounts().ToList();
            accounts = accounts.Where(a => !dbAccounts.Any(d => d.AccountName == a.AccountName)).ToList();

            return accounts;
        }

        private void AddAccounts(List<DBAccount> accountsToAdd)
        {
            foreach (DBAccount account in accountsToAdd)
            {
                _context.Add<DBAccount>(account);
            }
        }

        private async Task ModifyAccounts(List<DBAccount> allAccounts)
        {
            
            UpdateAccounts(GetUpdateList(allAccounts));
            AddAccounts(GetAccountsToAdd(allAccounts));
            DeleteDbAccounts(allAccounts);

            await _context.SaveChangesAsync();
        }

        private DBAccount GetDBAccount(string accountName)
        {
            var dbaccounts = GetDBAccounts();
            return dbaccounts.First(a => a.AccountName == accountName);
        }

        public bool IsAccountMaintained(IAccount account)
        {
            DBAccount dbAccount = GetDBAccount(account.AccountName);
            return dbAccount.MaintainedID;
        }
    }
}
