using System.Text.Json;
using ToolBox.Models;

namespace ToolBox.Services
{
    public class JsonHistoryService
    {
        public string HistoryJsonFileName { get; private set; }

        public JsonHistoryService(ServerType server) 
        {
            HistoryJsonFileName = FilePathService.LicenseManagerPath(server) + "history.json";
        }

        public void addDeletedAccount(Account account)
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
            using (var jsonFileReader = File.OpenText(HistoryJsonFileName))
            {
                return JsonSerializer.Deserialize<History>(jsonFileReader.ReadToEnd());
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
