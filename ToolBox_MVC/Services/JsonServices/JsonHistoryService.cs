using MFilesAPI;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.JsonServices
{
    public class JsonHistoryService : IAccountsHistoryHandler
    {
        private readonly IMFilesUsersHandler _usersHandler;

        public string HistoryJsonFileName { get; private set; }
        public ServerType Server { get; set; }

        public JsonHistoryService(ServerType server, IMFilesUsersHandler usersHandler) 
        {
            HistoryJsonFileName = FilePathService.LicenseManagerPath(server) + "history.json";
            Server = server;
            _usersHandler = usersHandler;
        }

        public void AddSuppressedAccount(string accountName)
        {
            

            ServerLoginAccountOperations loginOperations = _usersHandler.MFilesServerApp.LoginAccountOperations;

            Account account = new Account(loginOperations.GetLoginAccount(accountName));

            AddSuppressedAccount(account);
        }

        public void AddRestoredAccount(string accountName)
        {

            ServerLoginAccountOperations loginOperations = _usersHandler.MFilesServerApp.LoginAccountOperations;

            Account account = new Account(loginOperations.GetLoginAccount(accountName));

            AddRestoredAccount(account);
        }

        public void AddSuppressedAccount(Account account)
        {
            const int MAXDATES = 100;
            // Initialisation
            int index = 0;
            bool existingDate = false;
            TaskHistory history = GetHistory();
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);

            // Traitement
            foreach (TaskDate date in history.SuppressionDates)
            {
                if (date.Date == dateToday)
                {
                    existingDate = true;
                    break;
                }

                index += 1;
            }

            if (existingDate)
            {
                history.SuppressionDates[index].Accounts.Add(account);
                history.SuppressionDates[index].Hours.Add(TimeOnly.FromDateTime(DateTime.Now));
            }
            else
            {
                TaskDate date = new TaskDate();

                date.Date = dateToday;
                date.Accounts.Add(account);
                date.Hours.Add(TimeOnly.FromDateTime(DateTime.Now));

                history.SuppressionDates.Insert(0, date);
                if (history.SuppressionDates.Count > MAXDATES)
                {
                    history.SuppressionDates.RemoveAt(history.SuppressionDates.Count - 1);
                }
            }

            // Sortie
            SerializeHistory(history);
        }

        public void AddRestoredAccount(Account account)
        {
            const int MAXDATES = 100;
            // Initialisation
            int index = 0;
            bool existingDate = false;
            TaskHistory history = GetHistory();
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Now);

            // Traitement
            foreach (TaskDate date in history.RestorationDates)
            {
                if (date.Date == dateToday)
                {
                    existingDate = true;
                    break;
                }

                index += 1;
            }

            if (existingDate)
            {
                history.RestorationDates[index].Accounts.Add(account);
                history.RestorationDates[index].Hours.Add(TimeOnly.FromDateTime(DateTime.Now));
            }
            else
            {
                TaskDate date = new TaskDate();

                date.Date = dateToday;
                date.Accounts.Add(account);
                date.Hours.Add(TimeOnly.FromDateTime(DateTime.Now));

                history.RestorationDates.Insert(0, date);
                if (history.RestorationDates.Count > MAXDATES)
                {
                    history.RestorationDates.RemoveAt(history.RestorationDates.Count - 1);
                }
            }

            // Sortie
            SerializeHistory(history);
        }
        

        public TaskHistory GetHistory()
        {
            try
            {
                using (var jsonFileReader = File.OpenText(HistoryJsonFileName))
                {
                    return JsonSerializer.Deserialize<TaskHistory>(jsonFileReader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                return new TaskHistory();
            }
        }

        public void SerializeHistory(TaskHistory history)
        {
            File.Delete(HistoryJsonFileName);
            using (var outputStream = File.OpenWrite(HistoryJsonFileName))
            {
                JsonSerializer.Serialize(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    history
                );
            }
        }
    }
}
