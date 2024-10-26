using Microsoft.AspNetCore.Identity;
using RestfulLanding.Database;
using System.Security.Claims;

namespace RestfulLanding.Models {
    public static class ObjectivesManager {
        public static IQueryable<Objective> GetObjectives(ref readonly UserManager<UserModel> _userManager, ref readonly AppIdentityDbContext _context, ClaimsPrincipal User) {
            var currId = _userManager.GetUserId(User);
            var objectives = _context.Objectives.Where(t => t.userId == currId);

            return objectives;
        }

        public static ref IQueryable<Objective> SortObjectives(ref IQueryable<Objective> objectives, int SortBy) {
            switch (SortBy) {
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

            return ref objectives;
        }
    }
}
