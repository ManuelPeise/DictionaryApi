using Logic.Import.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Import.DI
{
    public static class ImportServiceRegistration
    {
        public static void RegisterImportServices(this IServiceCollection services)
        {
            services.AddScoped<IKaikkiParser, KaikkiParser>();
            services.AddScoped<IFileImporter, FileImporter>();
            services.AddScoped<IVocabularyImporter, VocabularyImporter>();
            services.AddScoped<IVocabularyValidation, VocabularyValidation>();
        }
    }
}
