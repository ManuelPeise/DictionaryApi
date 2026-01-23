using Logic.UserService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.UserService.DI
{
    public static class UserServiceRegistrations
    {
        public static void RegisterUserServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAdministration, UserAdministration>();
            services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
        }
    }
}
