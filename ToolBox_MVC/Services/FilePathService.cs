/*
 *          Author : Gabriel BOUJON
 *          Date : 2025-03-25
 *          
 *          Desc. : Service function to get file paths in the app structure
 * 
 */

namespace ToolBox.Services
{
    public enum ServerType
    {
        Prod,
        Cert,
        Test
    }
    public class FilePathService
    {
        /// <summary>
        /// Get the file path for files used in the LicenseManager based on which server (Prod,Cert,Test)
        /// </summary>
        /// <param name="server">The type of server to look for</param>
        /// <returns>The string for the file path : "wwwroot\\data\\[server]\\" </returns>
        public static string LicenseManagerPath(ServerType server)
        {
            string lmPath = "wwwroot\\data\\";
            switch (server)
            {
                case ServerType.Prod:
                    lmPath += "LicenseManagerProd\\";
                    break;
                case ServerType.Cert:
                    lmPath += "LicenseManagerCert\\";
                    break;
                default:
                    lmPath += "LicenseManagerTest\\";
                    break;
            }

            return lmPath;
        }
    }
}
