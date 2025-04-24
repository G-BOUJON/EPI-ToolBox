using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.DirectoryServices.AccountManagement;
using ToolBox_MVC.Models;

namespace ToolBox_MVC.Services.ActiveDirectory
{
    public interface IAdConnectorFactory
    {
        PrincipalContext CreatePrincipalContext(int adId);
    }

    public class AdConnectorFactory : IAdConnectorFactory
    {
        private readonly IADCredRepository _repository;

        public AdConnectorFactory(IADCredRepository credRepo) 
        {
            _repository = credRepo;
        }

        public PrincipalContext CreatePrincipalContext(int adId)
        {
            ADConnexionInfos credentials = _repository.GetADCredential(adId);
            return new PrincipalContext(
                contextType: ContextType.Domain,
                name: credentials.Domain,
                container: credentials.Container,
                userName: credentials.Username,
                password: credentials.Password
                );
        }
    }
}
