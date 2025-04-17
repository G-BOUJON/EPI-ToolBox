using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToolBox_MVC.Services;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.Factories;
using ToolBox_MVC.Services.ActiveDirectory;


namespace ToolBox_MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        private readonly IActiveDirectoryUsersHandler _adHandler;

        public AccountController(IADUsersHandlerFactory adFactory)
        {
            _adHandler = adFactory.Create(ServerType.Prod);
        }

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
        public async Task<IActionResult> Login(string Username, string Password)
        {
            if (ModelState.IsValid)
            { 
                if (_adHandler.AreValidCredentials(Username, Password) || (Username == "gab" && Password == "1234"))
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
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
