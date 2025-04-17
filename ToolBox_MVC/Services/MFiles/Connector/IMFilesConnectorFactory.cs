using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.MFiles.Connector
{
    public interface IMFilesConnectorFactory
    {
        IMFilesConnector CreateConnection(MFilesConnexionInfo connexionInfo);
    }

    public class MFConnectorFactory : IMFilesConnectorFactory
    {
        public MFConnectorFactory() { }

        public IMFilesConnector CreateConnection(MFilesConnexionInfo connexionInfo)
        {
            return new MFilesConnector(connexionInfo);
        }
    }
}
