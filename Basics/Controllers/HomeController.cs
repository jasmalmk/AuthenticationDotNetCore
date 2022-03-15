

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            var grandmaClaim = new List<Claim>() {
            new Claim(ClaimTypes.Name,"Noora"),
            new Claim(ClaimTypes.Email,"Noora@gg.com"),
            new Claim("Grandma.Says","veri nice Girls"),

            };
            var licenceClaim = new List<Claim>() {
            new Claim(ClaimTypes.Name,"Noorah MK"),
            new Claim(ClaimTypes.DateOfBirth,"13/10/2020"),
            new Claim("Driving Licence","A+"),

            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaim, "Grandma Identity");
            var licenceIdentity = new ClaimsIdentity(licenceClaim, "Grandma Identity");

            var userPrinciple = new ClaimsPrincipal(new[] { grandmaIdentity, licenceIdentity });
            HttpContext.SignInAsync(userPrinciple);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff()
        {
            //Do stuff


            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            await _authorizationService.AuthorizeAsync(User, customPolicy);

            return View("Index");
        }
    }
}
