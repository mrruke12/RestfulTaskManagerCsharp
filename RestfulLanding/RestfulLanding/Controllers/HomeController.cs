using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestfulLanding.Database;
using RestfulLanding.Models;
using System.Diagnostics;
using System.Security.AccessControl;

namespace RestfulLanding.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager) {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int SortBy = 0) {
            SortBy = Math.Clamp(SortBy, 0, 6);
            ViewBag.SortBy = SortBy;

            if (User.Identity.IsAuthenticated) {
                var objectives = ObjectivesManager.GetObjectives(_userManager, _context, User);
                ObjectivesManager.SortObjectives(ref objectives, SortBy);
                return View(await objectives.ToListAsync());
            }
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string lang, string returnUrl) {
            CookieOptions options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) };
            
            Response.Cookies.Delete("UserLanguage");
            Response.Cookies.Append("UserLanguage", lang, options);

            LocalizationManager.Set(lang);

            return Redirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
