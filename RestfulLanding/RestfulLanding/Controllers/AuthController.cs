using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestfulLanding.Database;

namespace RestfulLanding.Controllers {
    public class AuthController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<User> signInManager, UserManager<User> userManager) {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register(string ReturnUrl = "") {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister user, string ReturnUrl = "") {
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

            var existringUser = await _userManager.FindByNameAsync(user.Email);

            if (existringUser != null) {
                ModelState.AddModelError("Email", LocalizationManager.current["UserEmailAlreadyExist"]);
            }

            if (!ModelState.IsValid) return View(user);

            User newUser = new User {
                UserName = user.Email,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded) {
                ModelState.AddModelError("Email", LocalizationManager.current["RegistrationFailed"]);
                Console.WriteLine($"Registration of user {user.Email}, {user.Password} {result.ToString()}");
                return View(user);
            }

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Login", "Auth", new { ReturnUrl });
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl = "") {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin user, string ReturnUrl = "") {
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

            if (!result.Succeeded) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidLogin"]);
                return View(user);
            }

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
