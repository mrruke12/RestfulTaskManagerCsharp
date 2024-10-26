using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestfulLanding.Database;

namespace RestfulLanding.Controllers {
    public class ObjectiveController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppIdentityDbContext _context;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public ObjectiveController(ILogger<HomeController> logger, IConfiguration configuration, AppIdentityDbContext context, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager) {
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
            UserModel currentUser = await _userManager.GetUserAsync(User);

            Console.WriteLine("___________________________________________________________________________");
            Console.WriteLine(currentUser);

            if (!ObjectivesManipulations.ValidateNewObjective(ModelState, ref objective, ref currentUser)) return View(objective);

            await ObjectivesManipulations.PushNewObjective(_context, objective);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string ReturnUrl, int objectiveId) {
            var objective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            if (!await ObjectivesManipulations.DeleteObjective(_context, objective)) 
                throw new NullReferenceException($"The objective with id {objectiveId} either does not exist or couldn't be found while trying to delete it");

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
            if (!ObjectivesManipulations.ValidateEditObjective(ModelState, ref objective)) return View(objective);

            var existingObjective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == objectiveId);

            await ObjectivesManipulations.EditObjective(_context, objective, existingObjective);

            if (!string.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
}
