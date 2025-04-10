using System.Runtime.CompilerServices;
using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Services.Periodic
{
    public class PeriodicTaskTest : IPeriodicOperations
    {
        private readonly IConfigurationHandlerFactory _configFactory;
        public PeriodicTaskTest(IConfigurationHandlerFactory configFactory)
        {
            _configFactory = configFactory;
        }

        public async Task DoWork()
        {
            

            List<Task> serversTask = new List<Task>();

            IConfigurationHandler configHandler = null;
            foreach (ServerType server in Enum.GetValues(typeof(ServerType)))
            {
                configHandler = _configFactory.Create(server);
                if (RightHour(configHandler))
                {
                    serversTask.Add(SaySomethingAsync(configHandler));
                }
            }

            if (serversTask.Count > 0)
            {
                await Task.WhenAll(serversTask);
            }
            else
            {
                Console.WriteLine(string.Format("{0} : Aucune opération", DateTime.Now.ToString("HH:mm")));
            }
        }

        private bool RightHour(IConfigurationHandler configHandler)
        {
            
            TimeOnly targetHour = configHandler.GetConfiguration().Hour;
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

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

        private async Task SaySomethingAsync(IConfigurationHandler configHandler)
        {
            string consoleMessage = string.Format("{0} Opération sur serveur {1} : " + Environment.NewLine, DateTime.Now.ToString("HH:mm"), configHandler.Server);

            if (IsRestoreActive(configHandler))
            {
                consoleMessage += string.Format("\tRestoration sur serveur {0} activée à {1}" + Environment.NewLine, configHandler.Server, DateTime.Now.ToString("HH:mm"));
            }
            if (IsDeleteActive(configHandler))
            {
                consoleMessage += string.Format("\tSuppression sur serveur {0} activée à {1}" + Environment.NewLine, configHandler.Server, DateTime.Now.ToString("HH:mm"));
            }

            Console.WriteLine(consoleMessage);

        }
    }
}
