/*
 * 
 *      Auteur : Gabriel BOUJON
 *      Date : 2025-04-08
 *      
 *      Description : Classe contenant deux énumérateurs d'Account afin de mettre en place une sérialization et désérialization
 * 
 * 
 */
using MFilesAPI;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Areas.LicenseManager.Models
{
    public class Accounts
    {
        public List<Account> AccountsToDelete { get; set; }
        public List<Account> AccountsToRestore { get; set; }

        public Accounts() 
        {
            AccountsToDelete = new List<Account>();
            AccountsToRestore = new List<Account>();
        }

        public Accounts(List<Account> accountDelete, List<Account> accountRestore)
        {
            AccountsToDelete = accountDelete;
            AccountsToRestore = accountRestore;
        }


        public List<Account> GetAllAccounts()
        {
            List<Account> allAccounts = new List<Account>();

            allAccounts.AddRange(AccountsToDelete);
            allAccounts.AddRange(AccountsToRestore);

            return allAccounts;
        }

    }
}
