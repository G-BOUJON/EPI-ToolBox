using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.DirectoryServices.AccountManagement;
using System.Security.Authentication;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdConnectorFactory
    {
        /// <summary>
        /// Creates a new PrincipalContext based on the MFilesServer with the corresponding ID
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        /// <exception cref="PrincipalServerDownException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Creates a new PrincipalContext based on the MFilesServer with the corresponding ID
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        /// <exception cref="PrincipalServerDownException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public PrincipalContext CreatePrincipalContext(int serverId)
        {
            if (!allCredentials.ContainsKey(serverId))
            {
                throw new ArgumentException("No server with the corresponding ID");
            }

            ADCredential credentials = allCredentials[serverId];

            try
            {
                var principal = new PrincipalContext(
                contextType: ContextType.Domain,
                name: credentials.Domain,
                container: credentials.Container,
                userName: credentials.EncryptedUsername,
                password: credentials.EncryptedPassword
                );

                if (!principal.ValidateCredentials(credentials.EncryptedUsername, credentials.EncryptedPassword))
                {
                    throw new AuthenticationException("Invalid Credentials");
                }

                return principal;

            }
            catch (PrincipalServerDownException ex)
            {
                throw new PrincipalServerDownException(ex.Message);
            }

            
        }
    }
}
