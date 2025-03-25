using System.Text.Json;
using MFilesAPI;
using ToolBox.Models;

namespace ToolBox.Services.LicenseManagerCert
{
    public class JsonLoginAccountsService
    {
        private string JsonFileName
        {
            get { return "wwwroot\\data\\LicenseManagerCert\\loginAccounts.json"; }
        }

        public JsonLoginAccountsService()
        {
        }

        public IEnumerable<Account> getAccounts()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<Account[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public void updateList(List<LoginAccount> accounts)
        {
            List<Account> list = new List<Account>();
            foreach (LoginAccount account in accounts)
            {
                Account acnt = new Account(account);
                list.Add(acnt);
            }
            File.Delete(JsonFileName);
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Account>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    list
                );
            }
        }

        public void updateList2(List<LoginAccount> accounts)
        {
            File.Delete(JsonFileName);
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<LoginAccount>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    accounts
                );
            }
        }
    }
}
