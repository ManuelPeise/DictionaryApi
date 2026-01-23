using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Service.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class ApiAuthentication : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtTokenService = context.HttpContext.RequestServices.GetService<IJwtTokenService>();
            if (jwtTokenService == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var jwtModel = jwtTokenService.GetJwtOptions();

            if (jwtModel == null || string.IsNullOrEmpty(jwtModel.SecurityKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!ValidateJwtToken(token, jwtModel))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private bool ValidateJwtToken(string token, dynamic jwtModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtModel.SecurityKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false, // Set to true and provide ValidIssuer if needed
                    ValidateAudience = true,
                    ValidAudience = jwtModel.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                // Optionally log exception here
                return false;
            }
        }
    }
}
