using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace RestfulLanding.Middleware {
    public class ProfileMiddleware {
        private readonly RequestDelegate _next;

        public ProfileMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<UserModel> _userManager) {
            if (context.User.Identity.IsAuthenticated) {
                UserStatisticsManager.SetUser(await _userManager.GetUserAsync(context.User));
            }

            await _next(context);
        }
    }
}
