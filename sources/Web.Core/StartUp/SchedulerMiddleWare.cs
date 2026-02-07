using Shared.Enums;
using System.Security.Claims;

namespace Web.Core.StartUp
{
    public class SchedulerMiddleWare
    {
        private readonly RequestDelegate _next;

        public SchedulerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Schedule-Job", out var value) && value == "true")
            {
                var claims = new[] {
                new Claim("user_name", UserRoleEnum.MaintenanceUser.ToString()),
                new Claim("user_role", UserRoleEnum.MaintenanceUser.ToString())
            };
                var identity = new ClaimsIdentity(claims, "ScheduleJob");
                context.User = new ClaimsPrincipal(identity);
            }
            await _next(context);
        }
    }
}
