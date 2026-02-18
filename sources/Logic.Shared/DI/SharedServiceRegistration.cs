using Logic.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Shared.DI
{
    public static class SharedServiceRegistration
    {
        public static void RegisterSharedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<ILogicBase, LogicBase>();
        }
    }
}
