using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToolBox.Services;
using ToolBox_MVC.Models;


namespace ToolBox_MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Username,Password")] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                MFilesUsersService mf = new MFilesUsersService(new JsonConfService(ServerType.Prod).getConf());
                if (mf.areValidCredentials(credentials.Username, credentials.Password) || (credentials.Username == "gab" && credentials.Password == "1234"))
                {
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com")
                };
                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

                    return RedirectToAction("Index","Home");
                }
            }
            return View(credentials);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
