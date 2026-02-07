using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Enums;

namespace Service.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class UserRoleAuthentication : Attribute, IAuthorizationFilter
    {
        public UserRoleEnum RequiredRole { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "user_role");
            
            if (!Enum.TryParse<UserRoleEnum>(roleClaim?.Value, out var userRole) || userRole != RequiredRole)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
