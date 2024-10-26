using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestfulLanding.Database;
using System.Security.Policy;

namespace RestfulLanding.Controllers {
    public class ProfileController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public ProfileController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager) {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Info() {
            var usersObjectives = await ObjectivesManager.GetObjectives(_userManager, _context, User).ToListAsync();

            var statistics = new UserStatisticsModel();

            foreach (Objective objective in usersObjectives) {
                statistics.total++;
                switch (objective.status) {
                    case Status.done:
                        statistics.done++;
                        break;
                    case Status.inwork:
                        statistics.inwork++;
                        break;
                    default:
                        statistics.none++;
                        break;
                }
            }

            ViewBag.User = await _userManager.GetUserAsync(User);

            return View(statistics);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangeEmail() {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser.ToChangeEmailModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeAuthModel model) {
            ModelState.Remove("Password");
            if (!UserValidators.ValidateEmail(ModelState, model.Email)) return View(model);

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _userManager.SetEmailAsync(currentUser, model.Email);

            if (result.Succeeded) {
                currentUser.UserName = model.Email;
                currentUser.Email = model.Email;
            } else {
                ModelState.AddModelError("Email", LocalizationManager.current["ChangeEmailError"]);
                return View(model);
            }

            await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("Info", "Profile");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            var currentUser = await _userManager.GetUserAsync(User);
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangeAuthModel model) {
            ModelState.Remove("Email");
            if (!UserValidators.ValidatePassword(ModelState, model.Password)) return View(model);

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _userManager.RemovePasswordAsync(currentUser);

            if (result.Succeeded) {
                await _userManager.AddPasswordAsync(currentUser, model.Password);
            } else {
                ModelState.AddModelError("Email", LocalizationManager.current["ChangePasswordError"]);
                return View(model);
            }

            await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("Info", "Profile");
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteAccount() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int response) {
            var currentUser = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(currentUser);
           
            return RedirectToAction("Index", "Home");
        }
    }
}
