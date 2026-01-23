using Logic.Words.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Words.DI
{
    public static class WordServiceRegistration
    {
        public static void RegisterWordServices(this IServiceCollection services)
        {
            services.AddScoped<IKakkiWordService, KaikkiWordService>();
            services.AddScoped<IWordService, WordService>();
        }
    }
}
