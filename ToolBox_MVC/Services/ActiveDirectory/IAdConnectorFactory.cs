using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdConnectorFactory
    {
        PrincipalContext CreatePrincipalContext(int serverID);
    }

    public class AdConnectorFactory : IAdConnectorFactory
    {
        private readonly Dictionary<int,ADCredential> allCredentials;

        public AdConnectorFactory(IADCredentialService credRepo, IServerRepository servRepo) 
        {
            var allServer = Task.Run(servRepo.GetAllAsync).Result;

            allCredentials = new();
            foreach (var server in allServer)
            {
                allCredentials.Add(server.Id,Task.Run(() => credRepo.GetCredential(server.Id)).Result);
            }
        }

        public PrincipalContext CreatePrincipalContext(int serverId)
        {
            ADCredential credentials = allCredentials[serverId];
            return new PrincipalContext(
                contextType: ContextType.Domain,
                name: credentials.Domain,
                container: credentials.Container,
                userName: credentials.EncryptedUsername,
                password: credentials.EncryptedPassword
                );
        }
    }
}
