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
        /// Creates a new PrincipalContext based on the ActiveDirectory with the corresponding ID
        /// </summary>
        /// <param name="id">The local ID of the selected AD</param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        /// <exception cref="PrincipalServerDownException"></exception>
        /// <exception cref="ArgumentException"></exception>
        PrincipalContext CreatePrincipalContext(int id);
    }

    public class AdConnectorFactory : IAdConnectorFactory
    {
        private readonly Dictionary<int,Models.ActiveDirectory> allCredentials;

        public AdConnectorFactory(IADCredentialService credRepo, IGenericRepository<Models.ActiveDirectory> servRepo) 
        {
            var allServer = Task.Run(servRepo.GetAllAsync).Result;

            allCredentials = new();
            foreach (var server in allServer)
            {
                var copyServer = server.Clone();
                credRepo.UnprotectCredentials(copyServer);
                allCredentials.Add(copyServer.ID,copyServer);
            }
        }

        /// <summary>
        /// Creates a new PrincipalContext based on the ActiveDirectory with the corresponding ID
        /// </summary>
        /// <param name="id">The local ID of the selected AD</param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        /// <exception cref="PrincipalServerDownException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public PrincipalContext CreatePrincipalContext(int id)
        {


            Models.ActiveDirectory credentials = allCredentials[id];

            try
            {
                var principal = new PrincipalContext(
                contextType: ContextType.Domain,
                name: credentials.Name,
                container: credentials.Container,
                userName: credentials.EncryptedCredentials.Username,
                password: credentials.EncryptedCredentials.Password
                );

                if (!principal.ValidateCredentials(credentials.EncryptedCredentials.Username, credentials.EncryptedCredentials.Password))
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


    public class ValidateCredentialFactory : IAdConnectorFactory
    {
        private readonly IConfiguration _config;

        public ValidateCredentialFactory(IConfiguration config)
        {
            _config = config;
        }

        public PrincipalContext CreatePrincipalContext(int serverID)
        {
            return new PrincipalContext(ContextType.Domain, name: _config["DomainInfos:Domain"]);
        }
    }
}
