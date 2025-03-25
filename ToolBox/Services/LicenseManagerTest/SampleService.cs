using MFilesAPI;
using ToolBox.Models;

namespace ToolBox.Services.LicenseManagerTest
{
    public class SampleService
    {
        private readonly ILogger<SampleService> _logger;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public async Task DoSomethingAsync()
        {
            await Task.Delay(1000);

            if (rightHour())
            {
                MFilesUsersService mFilesUsersService = new MFilesUsersService();
                JsonConfService cs = new JsonConfService();
                JsonLoginAccountsService la = new();

                if (cs.getConf().active)
                {
                    List<LoginAccount> filteredAccounts = filterMaintainedAccounts(mFilesUsersService.getList(cs.getConf().groups));
                    deleteLicenses(filteredAccounts);
                    la.updateList(mFilesUsersService.getList(cs.getConf().groups));
                    Console.WriteLine($"Deleting the licenses for LicenseManagerTest at {DateTime.Now.ToString("HH:mm")}");
                }
                else
                {
                    Console.WriteLine($"Updating the list for LicenseManagerTest at {DateTime.Now.ToString("HH:mm")}");
                    la.updateList(mFilesUsersService.getList(cs.getConf().groups));
                }

                await Task.Delay(60000);
            }
        }

        public bool rightHour()
        {
            // Initialisation
            JsonConfService cs = new JsonConfService();
            string hour = cs.getConf().hour;
            bool validHour = false;

            // Traitement
            if (hour == DateTime.Now.ToString("HH:mm"))
            {
                validHour = true;
            }

            // Sortie
            return validHour;
        }

        private void deleteLicenses(List<LoginAccount> loginAccounts)
        {
            MFilesUsersService mfilesUsersService = new MFilesUsersService();
            foreach (LoginAccount account in loginAccounts)
            {
                mfilesUsersService.deleteLicence(account);
            }
        }

        public List<LoginAccount> filterMaintainedAccounts(List<LoginAccount> accounts)
        {
            // Initialisation
            List<LoginAccount> filteredAccounts = new List<LoginAccount>();
            JsonConfService confService = new JsonConfService();
            Config conf = confService.getConf();
            bool maintain = false;

            // Traitement
            foreach (LoginAccount account in accounts)
            {
                foreach (string username in conf.maintainedAccounts)
                {
                    if (account.Equals(username))
                    {
                        maintain = true; break;
                    }
                }

                if (!maintain)
                {
                    filteredAccounts.Add(account);
                }

                maintain = false;
            }

            // Sortie
            return filteredAccounts;
        }
    }
}
