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
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<User> signInManager, UserManager<User> userManager) {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int FilterBy = 0) {
            FilterBy = Math.Clamp(FilterBy, 0, 6);
            ViewBag.FilterBy = FilterBy;
            if (User.Identity.IsAuthenticated) {
                var currId = _userManager.GetUserId(User);
                var objectives = _context.Objectives.Where(t => t.userId == currId);
                
                switch (FilterBy) {
                    case 1:
                        objectives = objectives.OrderBy(o => (int)o.status);
                        break;
                    case 2:
                        objectives = objectives.OrderBy(o => (int)o.priority);
                        break;
                    case 3:
                        objectives = objectives.OrderByDescending(o => (int)o.priority);
                        break;
                    case 4:
                        objectives = objectives.OrderBy(o => (int)o.urgency);
                        break;
                    case 5:
                        objectives = objectives.OrderByDescending(o => (int)o.urgency);
                        break;
                    case 6:
                        objectives = objectives.OrderBy(o => o.due);
                        break;
                    default:
                        objectives = objectives.OrderBy(o => o.description);
                        break;
                }

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

        [Authorize]
        [HttpGet]
        public IActionResult Profile() {
            return View();
        }
    }
}
