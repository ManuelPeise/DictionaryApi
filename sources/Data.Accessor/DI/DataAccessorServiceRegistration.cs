using Data.Accessor.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Accessor.DI
{
    public static class DataAccessorServiceRegistration
    {
        public static void RegisterDataAccessorServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVocabularyUnitOfWork, VocabularyUnitOfWork>();
        }
    }
}
