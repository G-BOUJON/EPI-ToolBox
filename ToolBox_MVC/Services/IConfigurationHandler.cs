/*
 * 
 *      Auteur : Gabriel BOUJON
 *      Date : 2025-04-09
 *      
 *      Desc. : Interface pour services de gestion de configuration
 */
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services
{
    public interface IConfigurationHandler
    {
        Config GetConfiguration();
        void UpdateConfiguration(Config configuration);
        List<string> GetMaintainedAccounts();

        void AddMaintainedAccount(string username);
        void RemoveMaintainedAccount(string username);
    }
}
