using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestfulLanding.Database;
using RestfulLanding.Models;
using System.Diagnostics;

namespace RestfulLanding.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        //private readonly AppDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, SignInManager<User> signInManager, UserManager<User> userManager) {
            _logger = logger;
            _configuration = configuration;
            //_context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index() {
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

        [Authorize]
        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin user) {
            if (string.IsNullOrEmpty(user.Email) || !System.Text.RegularExpressions.Regex.IsMatch(user.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidEmail"]);
            }

            if (string.IsNullOrEmpty(user.Password)) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordRequired"]);
            }

            if (user.Password != null && user.Password.Length < 6 || user.Password != null && user.Password.Length > 30) {
                ModelState.AddModelError("Password", LocalizationManager.current["InvalidPassword"]);
            }

            if (!ModelState.IsValid) return View(user);

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, isPersistent: true, lockoutOnFailure: false);

            Console.WriteLine(user.Password);

            if (!result.Succeeded) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidLogin"]);
                return View(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister user) {
            if (string.IsNullOrEmpty(user.Email) || !System.Text.RegularExpressions.Regex.IsMatch(user.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidEmail"]);
            }

            if (string.IsNullOrEmpty(user.Password)) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordRequired"]);
            }

            if (user.Password != null && user.Password.Length < 6) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordInsufficientLength"] + "6");
            }

            if (user.Password != null && user.Password.Length > 30) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordOverheapingLength"] + "30");
            }

            if (user.Password != null && !System.Text.RegularExpressions.Regex.IsMatch(user.Password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()]).+$")) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordIncorrect"]);
            }

            if (string.IsNullOrEmpty(user.PasswordConfirm)) {
                ModelState.AddModelError("PasswordConfirm", LocalizationManager.current["PasswordConfirmRequired"]);
            }

            if (user.PasswordConfirm != null && user.PasswordConfirm != user.Password) {
                ModelState.AddModelError("PasswordConfirm", LocalizationManager.current["PasswordsDontMatch"]);
            }

            //if (await _context.Users.AnyAsync(u => u.Email == user.Email)) {
            //    ModelState.AddModelError("Email", LocalizationManager.current["UserEmailAlreadyExist"]);
            //}

            if (!ModelState.IsValid) return View(user);

            User newUser = new User {
                UserName = user.Email,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            Console.WriteLine("reg " + result);
            //newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            //await _context.Users.AddAsync(newUser);
            //await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Home");
        }
    }
}
