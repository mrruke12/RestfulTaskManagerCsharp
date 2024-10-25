using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfulLanding.Database;

namespace RestfulLanding.Controllers {
    public class ObjectiveController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public ObjectiveController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<User> signInManager, UserManager<User> userManager) {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Objective objective) {
            if (string.IsNullOrEmpty(objective.description)) {
                ModelState.AddModelError("description", LocalizationManager.current["ObjectiveDescriptionRequired"]);
            }

            if (objective.due == null || objective.due == default || string.IsNullOrEmpty(objective.due.ToString())) {
                objective.due = DateTime.MaxValue;
            }

            objective.user = await _userManager.GetUserAsync(User);
            objective.userId = objective.user.Id;

            ModelState.Remove("user");
            ModelState.Remove("userId");

            foreach (var problem in ModelState) foreach (var a in problem.Value.Errors) Console.WriteLine(a.ErrorMessage);

            if (!ModelState.IsValid) return View(objective);

            await _context.Objectives.AddAsync(objective);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string ReturnUrl, int objectiveId) {
            var objective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective != null) {
                _context.Objectives.Remove(objective);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string ReturnUrl, int objectiveId) {
            var objective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (objective != null) {
                if (objective.due == DateTime.MaxValue) objective.due = null;
                ViewBag.ReturnUrl = ReturnUrl;
                ViewBag.objectiveId = objectiveId;
                return View(objective);
            }

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Objective objective, string ReturnUrl, int objectiveId) {
            if (string.IsNullOrEmpty(objective.description)) {
                ModelState.AddModelError("description", LocalizationManager.current["ObjectiveDescriptionRequired"]);
            }

            if (objective.due == null || objective.due == default || string.IsNullOrEmpty(objective.due.ToString())) {
                objective.due = DateTime.MaxValue;
            }

            ModelState.Remove("user");
            ModelState.Remove("userId");

            foreach (var problem in ModelState) foreach (var a in problem.Value.Errors) Console.WriteLine(a.ErrorMessage);

            if (!ModelState.IsValid) return View(objective);

            var existing = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            existing.description = objective.description;
            existing.priority = objective.priority;
            existing.urgency = objective.urgency;
            existing.due = objective.due;
            existing.status = objective.status;

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
}
