using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToolBox.Models;
using ToolBox.Services.LicenseManagerTest;

namespace ToolBox.Controllers.LicenseManagerTest
{
    [Route("LicenseManagerTest/api/accounts")]
    [ApiController]
    public class LoginAccountsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string token)
        {
            JsonConfService cf = new JsonConfService();
            Config config = cf.getConf();

            if (config.apiKey == token)
            {
                JsonLoginAccountsService la = new JsonLoginAccountsService();
                MFilesUsersService mf = new MFilesUsersService();
                la.updateList(mf.getList(cf.getConf().groups));

                return Ok(la.getAccounts().ToList());
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        [HttpDelete("delete")]
        public ActionResult Delete([FromQuery] string token, [FromQuery] string username)
        {
            JsonConfService jsonConfService = new JsonConfService();
            Config config = jsonConfService.getConf();
            MFilesUsersService mFilesUsersService = new MFilesUsersService();

            if(config.apiKey == token)
            {
                Console.WriteLine("Delete Licence for " + username);
                List<Account> accounts = this.getAccounts();

                foreach (Account account in accounts)
                {
                    if (account.UserName == username)
                    {
                        mFilesUsersService.deleteLicence(mFilesUsersService.convertToLoginAccount(account));break;
                    }
                }

                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authentication token not recognized");
            return BadRequest(ModelState);
        }

        public List<Account> getAccounts()
        {
            JsonLoginAccountsService la = new JsonLoginAccountsService();
            JsonConfService confService = new JsonConfService();
            List<Account> nonExistingAccounts = la.getAccounts().ToList();
            Config config = confService.getConf();

            return nonExistingAccounts.ToList();
        }

        [HttpDelete("deleteAll")]
        public ActionResult deleteAll([FromQuery] string token)
        {
            JsonConfService jsonConfService = new JsonConfService();
            Config config = jsonConfService.getConf();
            MFilesUsersService mFilesUsersService = new MFilesUsersService();

            if (config.apiKey == token)
            {
                Console.WriteLine("Delete all licence without the maintained ones");

                List<Account> accounts = this.getAccounts();

                foreach (Account account in accounts)
                {

                    mFilesUsersService.deleteLicence(mFilesUsersService.convertToLoginAccount(account));
                    
                }

                return Ok();
            }

            ModelState.AddModelError("Authentication", "Authetication token not recognized");
            return BadRequest(ModelState);
        }
    }
}
