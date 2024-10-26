using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestfulLanding.Database;

namespace RestfulLanding.Controllers {
    public class AuthController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public AuthController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager) {
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
            var existingUser = await _userManager.FindByNameAsync(user.Email);

            if (!UserValidators.UserRegisterValidate(ModelState, ref user, ref existingUser)) return View(user);

            var bdRequestResult = await _userManager.CreateAsync(user.ToUser(), user.Password);
            
            if (!bdRequestResult.Succeeded) {
                ModelState.AddModelError("Email", LocalizationManager.current["RegistrationFailed"]);
                Console.WriteLine($"Registration of user {user.Email}, {user.Password} {bdRequestResult.ToString()}"); // logging will be implemented
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
            if (!UserValidators.UserLoginValidator(ModelState, ref user)) return View(user);

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, isPersistent: true, lockoutOnFailure: false);
            
            if (!result.Succeeded) {
                ModelState.AddModelError("Email", LocalizationManager.current["InvalidLogin"]);
                return View(user);
            }

            UserStatisticsManager.SetUser(await _userManager.GetUserAsync(User));

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ConfirmPassword(string Email, string nextAction) {
            return View(new ConfirmPasswordModel { Action = nextAction, Email = Email, Password = ""});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmPassword(ConfirmPasswordModel model) {
            if (!await _userManager.CheckPasswordAsync(await _userManager.GetUserAsync(User), model.Password)) {
                ModelState.AddModelError("Password", LocalizationManager.current["PasswordConfirmError"]);
                return View(model);
            }
            return RedirectToAction(model.Action, "Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            UserStatisticsManager.ResetUser();
            return RedirectToAction("Index", "Home");
        }
    }
}
