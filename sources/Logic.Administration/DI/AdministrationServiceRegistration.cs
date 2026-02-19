using Logic.Administration.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Administration.DI
{
    public static class AdministrationServiceRegistration
    {
        public static void RegisterAdministrationServices(this IServiceCollection services)
        {
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IVocabularySyncronization, VocabularySyncronization>();
        }
    }
}
