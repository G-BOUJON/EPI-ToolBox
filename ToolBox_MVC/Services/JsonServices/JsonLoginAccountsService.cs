/* Modified by GBOUJON 2025-03-27*/
using System.Collections.Generic;
using System.Text.Json;
using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Models;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.JsonServices
{
    public enum LicenseManagerOperation
    {
        Suppression,
        Restoration
    }
    public class JsonLoginAccountsService
    {
        

        public string JsonFileName
        {
            get;
            private set;
        }

        public JsonLoginAccountsService(ServerType server)
        {
            JsonFileName = FilePathService.LicenseManagerPath(server) + "loginAccounts.json";
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<IAccount[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public List<Account> GetRestoredAccounts()
        {
            return GetAccounts_V2().AccountsToRestore.ToList();
        }

        public List<Account> GetDeletedAccounts()
        {
            return GetAccounts_V2().AccountsToDelete.ToList();
        }

        public Accounts GetAccounts_V2()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<Accounts>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public void UpdateRestoreList(List<LoginAccount> restoreAccounts)
        {
            List<Account> list = new List<Account>();
            foreach (LoginAccount account in restoreAccounts)
            {
                Account acnt = new Account(account);
                list.Add(acnt);
            }
            Accounts accounts = GetAccounts_V2();
            accounts.AccountsToRestore = list;
            UpdateAllAccounts(accounts);
        }

        public void UpdateDeleteList(List<LoginAccount> deleteAccounts)
        {
            List<Account> list = new List<Account>();
            foreach (LoginAccount account in deleteAccounts)
            {
                Account acnt = new Account(account);
                list.Add(acnt);
            }
            Accounts accounts = GetAccounts_V2();
            accounts.AccountsToDelete = list;
            UpdateAllAccounts(accounts);
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

        public void UpdateAllAccounts(Accounts accounts)
        {
            File.Delete(JsonFileName);
            using (var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize(new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                accounts);
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
