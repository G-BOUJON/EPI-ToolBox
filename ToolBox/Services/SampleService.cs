using MFilesAPI;
using ToolBox.Models;

namespace ToolBox.Services
{
    public class SampleService
    {
        private readonly ILogger<SampleService> _logger;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public async Task DoSomethingAsync(ServerType server)
        {
            await Task.Delay(1000);

            if (rightHour(server))
            {
                MFilesUsersService mFilesUsersService = new MFilesUsersService(server);
                JsonConfService cs = new JsonConfService(server);
                JsonLoginAccountsService la = new(server);

                if (cs.getConf().active)
                {
                    List<LoginAccount> filteredAccounts = filterMaintainedAccounts(mFilesUsersService.getList(cs.getConf().groups),server);
                    deleteLicenses(filteredAccounts,server);
                    la.updateList(mFilesUsersService.getList(cs.getConf().groups));
                    Console.WriteLine($"Deleting the licenses for LicenseManagerCert at {DateTime.Now.ToString("HH:mm")}");
                }
                else
                {
                    Console.WriteLine($"Updating the list for LicenseManagerCert at {DateTime.Now.ToString("HH:mm")}");
                    la.updateList(mFilesUsersService.getList(cs.getConf().groups));
                }

                await Task.Delay(60000);
            }
        }

        public bool rightHour(ServerType s)
        {
            // Initialisation
            JsonConfService cs = new JsonConfService(s);
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

        private void deleteLicenses(List<LoginAccount> loginAccounts,ServerType server)
        {
            MFilesUsersService mfilesUsersService = new MFilesUsersService(server);
            foreach (LoginAccount account in loginAccounts)
            {
                mfilesUsersService.deleteLicence(account);
            }
        }

        public List<LoginAccount> filterMaintainedAccounts(List<LoginAccount> accounts,ServerType server)
        {
            // Initialisation
            List<LoginAccount> filteredAccounts = new List<LoginAccount>();
            JsonConfService confService = new JsonConfService(server);
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
