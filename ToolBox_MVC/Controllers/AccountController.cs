using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToolBox_MVC.Services;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Repositories;
using System.DirectoryServices.Protocols;


namespace ToolBox_MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        private readonly IAdService _adHandler;
        private readonly IServerRepository _serverRepo;

        public AccountController(IAdService adService,IServerRepository serverRepository)
        {
            _adHandler = adService;
            _serverRepo = serverRepository;
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
            var server = (await _serverRepo.GetAllAsync()).First();

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "Veuillez remplir les champs";
                return View();
            }
            bool adCheckResult = false;
            if (_adHandler.TryConnection(server.Id) == ADConnectionResult.Success)
            {
                try
                {
                    adCheckResult = _adHandler.AreValidCredentials(server.Id, Username, Password);
                }
                catch (LdapException)
                {
                    adCheckResult = false;
                }
            }
            if (adCheckResult || (Username == "gab" && Password == "1234")) // Replace second condition with master password check
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
            
            ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe invalide"; 
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
