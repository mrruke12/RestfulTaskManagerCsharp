using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RestfulLanding.Database;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace RestfulLanding.Models {
    public static class ObjectivesManipulations {
        public static bool ValidateNewObjective(ModelStateDictionary ModelState, ref Objective objective, ref UserModel currentUser) {
            if (string.IsNullOrEmpty(objective.description)) {
                ModelState.AddModelError("description", LocalizationManager.current["ObjectiveDescriptionRequired"]);
            }

            if (objective.due == null || objective.due == default || string.IsNullOrEmpty(objective.due.ToString())) {
                objective.due = DateTime.MaxValue;
            }

            objective.user = currentUser;
            objective.userId = currentUser.Id;

            ModelState.Remove("user");
            ModelState.Remove("userId");

            return ModelState.IsValid;
        }

        public static bool ValidateEditObjective(ModelStateDictionary ModelState, ref Objective objective) {
            if (string.IsNullOrEmpty(objective.description)) {
                ModelState.AddModelError("description", LocalizationManager.current["ObjectiveDescriptionRequired"]);
            }

            if (objective.due == null || objective.due == default || string.IsNullOrEmpty(objective.due.ToString())) {
                objective.due = DateTime.MaxValue;
            }

            ModelState.Remove("user");
            ModelState.Remove("userId");

            return ModelState.IsValid;
        }

        public static async Task<bool> PushNewObjective(AppIdentityDbContext _context, Objective objective) {
            await _context.Objectives.AddAsync(objective);
            await _context.SaveChangesAsync();

            await UserStatisticsManager.IncreaseTotal(_context);
            if (objective.status == Status.done) await UserStatisticsManager.IncreaseDone(_context);

            return true;
        }

        public static async Task<bool> DeleteObjective(AppIdentityDbContext _context, Objective objective) {
            if (objective != null) {
                await UserStatisticsManager.DecreaseTotal(_context);
                if (objective.status == Status.done) await UserStatisticsManager.DecreaseDone(_context);
                _context.Objectives.Remove(objective);
                await _context.SaveChangesAsync();
            } else return false;

            return true;
        }

        public static async Task<bool> EditObjective(AppIdentityDbContext _context, Objective objective, Objective existingObjective) {
            if (existingObjective.status == Status.done && objective.status != Status.done) await UserStatisticsManager.DecreaseDone(_context);
            if (existingObjective.status != Status.done && objective.status == Status.done) await UserStatisticsManager.IncreaseDone(_context);

            existingObjective.description = objective.description;
            existingObjective.priority = objective.priority;
            existingObjective.urgency = objective.urgency;
            existingObjective.due = objective.due;
            existingObjective.status = objective.status;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
