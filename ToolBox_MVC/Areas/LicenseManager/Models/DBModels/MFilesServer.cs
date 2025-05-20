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
        public string Domain { get; set; }
        public TimeOnly SyncTime { get; set; }
        

        public ADCredential ADCredential { get; set; }
        public MFilesCredential MfCredential { get; set; }
        /// <summary>
        /// Status of activation of each automatic operations that can be performed on this server
        /// </summary>
        public AutomaticOperations AutomaticOP { get; set; }
        

        public MFilesServer() { }

        public MFilesServer Clone()
        {
            return new MFilesServer
            {
                Id = this.Id,
                Name = this.Name,
                NetworkAddress = this.NetworkAddress,
                EndPoint = this.EndPoint,
                ProtocolSequence = this.ProtocolSequence,
                VaultGuid = this.VaultGuid,
                Domain = this.Domain,
                SyncTime = this.SyncTime,
                ADCredential = this.ADCredential.Clone(),
                MfCredential = this.MfCredential.Clone(),
                AutomaticOP = this.AutomaticOP.Clone()
            };
        }
    }
}
