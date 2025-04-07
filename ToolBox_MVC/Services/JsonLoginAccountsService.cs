/* Modified by GBOUJON 2025-03-27*/
using System.Collections.Generic;
using System.Text.Json;
using MFilesAPI;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public enum LicenseManagerOperation
    {
        Suppression,
        Restoration
    }
    public class JsonLoginAccountsService
    {
        private Dictionary<LicenseManagerOperation,string> JsonFileNames = new Dictionary<LicenseManagerOperation,string>() { 
            { LicenseManagerOperation.Suppression,"supLoginAccounts.json"},
            { LicenseManagerOperation.Restoration,"resLoginAccounts.json" }
        };

        public string JsonFileName
        {
            get;
            private set;
        }

        public JsonLoginAccountsService(ServerType server, LicenseManagerOperation operation)
        {
            JsonFileName = FilePathService.LicenseManagerPath(server) + JsonFileNames[operation];
        }

        public IEnumerable<Account> GetAccounts()
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

        public void UpdateList(List<LoginAccount> accounts)
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
