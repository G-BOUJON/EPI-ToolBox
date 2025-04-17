using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.DB;
using System.Linq;

namespace ToolBox_MVC.Areas.LicenseManager.Services
{

    public interface ILicenseMangagerService
    {

    }

    public class LicenseManager : ILicenseMangagerService
    {
        private readonly IAccountsRepository _accountsRepo;
        private readonly IGroupRepository _groupRepo;


        public LicenseManager(IAccountsRepository accountsRepo)
        {
            _accountsRepo = accountsRepo;
        }

        
    }
}
