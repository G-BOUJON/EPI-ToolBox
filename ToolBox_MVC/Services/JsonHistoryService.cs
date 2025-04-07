using MFilesAPI;
using System.DirectoryServices.ActiveDirectory;
using System.Text.Json;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public class JsonHistoryService
    {
        public Dictionary<LicenseManagerOperation,string> HISTORY_JSON = new Dictionary<LicenseManagerOperation, string>()
        { {LicenseManagerOperation.Suppression, "supHistory.json" },
            {LicenseManagerOperation.Restoration, "resHistory.json" }};

        public string HistoryJsonFileName { get; private set; }
        public ServerType Server { get; set; }

        public JsonHistoryService(ServerType server, LicenseManagerOperation operation) 
        {
            HistoryJsonFileName = FilePathService.LicenseManagerPath(server) + HISTORY_JSON[operation];
            Server = server;
        }

        public void AddAccount(string accountName)
        {
            MFilesUsersService mFilesUsers = new MFilesUsersService(new JsonConfService(Server).GetConf());

            ServerLoginAccountOperations loginOperations = mFilesUsers.mfServerApplication.LoginAccountOperations;

            Account account = new Account(loginOperations.GetLoginAccount(accountName));

            AddAccount(account);
        }

        public void AddAccount(Account account)
        {
            const int MAXDATES = 100;
            // Initialisation
            int index = 0;
            bool existingDate = false;
            History history = getHistory();
            string dateToday = DateTime.Now.ToShortDateString();

            // Traitement
            foreach (Date date in history.dates)
            {
                if (date.date == dateToday)
                {
                    existingDate = true;
                    break;
                }

                index += 1;
            }

            if (existingDate)
            {
                history.dates[index].deletedAccounts.Add(account);
                history.dates[index].hour.Add(DateTime.Now.ToString("h:mm:ss tt"));
            }
            else
            {
                Date date = new Date();

                date.date = dateToday;
                date.deletedAccounts.Add(account);
                date.hour.Add(DateTime.Now.ToString("h:mm:ss tt"));

                history.dates.Insert(0, date);
                if (history.dates.Count > MAXDATES)
                {
                    history.dates.RemoveAt(history.dates.Count - 1);
                }
            }

            // Sortie
            serializeHistory(history);
        }

        public History getHistory()
        {
            try
            {
                using (var jsonFileReader = File.OpenText(HistoryJsonFileName))
                {
                    return JsonSerializer.Deserialize<History>(jsonFileReader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                return new History();
            }
        }

        public void serializeHistory(History history)
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
