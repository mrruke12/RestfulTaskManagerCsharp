using RestfulLanding.Database;

namespace RestfulLanding.Models {
    public static class UserStatisticsManager {
        private static UserModel? user { get; set; } = null;
        public static bool isSet { get; set; } = false;
        
        public static void SetUser(UserModel newUser) {
            user = newUser;
            isSet = true;
        }

        public static void ResetUser() {
            user = null;
            isSet = false;
        }

        public static async Task<bool> IncreaseDone(AppIdentityDbContext _context) {
            if (isSet) {
                user.Completed++;
                await _context.SaveChangesAsync();
            } else throw new NullReferenceException("User is not set, but program tries to increase user's done objectives' counter");
            return true;
        }

        public static async Task<bool> DecreaseDone(AppIdentityDbContext _context) {
            if (isSet) {
                user.Completed--;
                await _context.SaveChangesAsync();
            } else throw new NullReferenceException("User is not set, but program tries to decrease user's done objectives' counter");
            return true;
        }

        public static async Task<bool> IncreaseTotal(AppIdentityDbContext _context) {
            if (isSet) {
                user.Total++;
                await _context.SaveChangesAsync();
            } else throw new NullReferenceException("User is not set, but program tries to increase user's Total objectives' counter");
            return true;
        }

        public static async Task<bool> DecreaseTotal(AppIdentityDbContext _context) {
            if (isSet) {
                user.Total--;
                await _context.SaveChangesAsync();
            } else throw new NullReferenceException("User is not set, but program tries to decrease user's Total objectives' counter");
            return true;
        }
    }
}
