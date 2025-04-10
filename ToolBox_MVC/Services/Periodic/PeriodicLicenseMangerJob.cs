
using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Services.Periodic
{
    public class PeriodicLicenseMangerJob : IPeriodicOperations
    {
        private readonly IAccountsHistoryHandlerFactory _historyFactory;
        private readonly IConfigurationHandlerFactory _configFactory;
        private readonly IMFilesUsersHandlerFactory _mfilesUsersFactory;
        private readonly IAccountsListHandlerFactory _accountsFactory;

        private readonly ILogger _logger;

        public PeriodicLicenseMangerJob(IAccountsHistoryHandlerFactory historyFactory, IConfigurationHandlerFactory configFactory, IMFilesUsersHandlerFactory mfilesUsersFactory, IAccountsListHandlerFactory accountsFactory, ILogger<PeriodicLicenseMangerJob> logger)
        {
            _historyFactory = historyFactory;
            _configFactory = configFactory;
            _mfilesUsersFactory = mfilesUsersFactory;
            _accountsFactory = accountsFactory;
            _logger = logger;
        }

        public async Task DoWork()
        {
            List<Task> serverTasks = new List<Task>();
            List<Task> refreshTasks = new List<Task>();
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

            IConfigurationHandler configHandler = null;
            IMFilesUsersHandler mFilesUsersHandler = null;
            IAccountsHistoryHandler accountsHistoryHandler = null;
            IAccountsListHandler listHandler = null;

            foreach(ServerType server in Enum.GetValues(typeof(ServerType)))
            {
                configHandler = _configFactory.Create(server);
                if (RightHour(configHandler,currentTime))
                {
                    _logger.LogInformation(string.Format("{0} : Opérations sur le serveur {1}",DateTime.Now.ToString("HH:mm"), server));
                    mFilesUsersHandler = _mfilesUsersFactory.Create(server);
                    accountsHistoryHandler = _historyFactory.Create(server);
                    listHandler = _accountsFactory.Create(server);
                    
                    if (IsDeleteActive(configHandler))
                    {
                        serverTasks.Add(DeleteAccountsAsync(mFilesUsersHandler, accountsHistoryHandler));
                    }
                    if (IsRestoreActive(configHandler))
                    {
                        serverTasks.Add(RestoreAccountsAsync(mFilesUsersHandler, accountsHistoryHandler));
                    }

                    
                    refreshTasks.Add(UpdateAccountsAsync(mFilesUsersHandler,listHandler));
                }
            }
            if (serverTasks.Count > 0)
            {
                await Task.WhenAll(serverTasks);
            }
            if (refreshTasks.Count > 0)
            {
                await Task.WhenAll(refreshTasks);
            }
            _logger.LogInformation(string.Format("{0} : job terminé - {1} opérations exécutées", DateTime.Now.ToString("HH:mm"), (serverTasks.Count + refreshTasks.Count).ToString()));
        }



        private bool RightHour(IConfigurationHandler configHandler, TimeOnly currentTime)
        {

            TimeOnly targetHour = configHandler.GetConfiguration().Hour;

            return (targetHour.Hour == currentTime.Hour && currentTime.Minute == targetHour.Minute);
        }

        private bool IsDeleteActive(IConfigurationHandler configHandler)
        {

            return configHandler.GetConfiguration().ActiveSuppression;
        }

        private bool IsRestoreActive(IConfigurationHandler configHandler)
        {

            return configHandler.GetConfiguration().ActiveRestauration;
        }

        private async Task UpdateAccountsAsync(IMFilesUsersHandler mfilesHandler, IAccountsListHandler accountHandler)
        {
            Accounts accounts = new Accounts(Account.ConvertLoginAccountList(mfilesHandler.GetSuppressionList()), Account.ConvertLoginAccountList(mfilesHandler.GetRestorationList()));
            accountHandler.UpdateAccounts(accounts);
            _logger.LogInformation(string.Format("{0} : Rafraichissement des comptes du serveur {1}", DateTime.Now.ToString("HH:mm"), mfilesHandler.Configuration.VaultCredentials.NetworkAddress));
        }

        private async Task DeleteAccountsAsync(IMFilesUsersHandler mfilesHandler, IAccountsHistoryHandler historyHandler)
        {
            List<LoginAccount> accounts = mfilesHandler.GetSuppressionList();
            int iteration = 0;
            foreach (LoginAccount account in accounts)
            {
                if (!mfilesHandler.Configuration.MaintainedAccounts.Contains(account.UserName))
                {
                    mfilesHandler.DeleteAccountLicense(account);
                    historyHandler.AddSuppressedAccount(account.AccountName);
                    iteration += 1;
                }
            }

            _logger.LogInformation(string.Format("{0} : Suppression de {1} licences", DateTime.Now.ToString("HH:mm"), iteration));
        }

        private async Task RestoreAccountsAsync(IMFilesUsersHandler mfilesHandler, IAccountsHistoryHandler historyHandler)
        {
            List<LoginAccount> accounts = mfilesHandler.GetRestorationList();
            int iteration = 0;
            foreach (LoginAccount account in accounts)
            {
                if (!mfilesHandler.Configuration.MaintainedAccounts.Contains(account.UserName))
                {
                    mfilesHandler.RestoreAccountLicense(account);
                    historyHandler.AddRestoredAccount(account.AccountName);
                    iteration += 1;
                }
            }
            _logger.LogInformation(string.Format("{0} : Restauration de {1} licences", DateTime.Now.ToString("HH:mm"), iteration));
        }


    }
}
