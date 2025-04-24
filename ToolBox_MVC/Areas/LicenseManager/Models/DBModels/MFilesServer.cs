using Microsoft.CodeAnalysis.CSharp.Syntax;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Areas.LicenseManager.Models.DBModels
{
    public class MFilesServer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NetworkAddress { get; set; }
        public string EndPoint {  get; set; }
        public string ProtocolSequence { get; set; }
        public string VaultGuid { get; set; }
        public int ADCredentialId { get; set; }

        public MFilesCredential Credential { get; set; }
        

        public MFilesServer() { }
    }
}
