using Logic.Parsing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Parsing.DI
{
    public static class ParsingSericeRegistrations
    {
        public static void RegisterParsingServices(this IServiceCollection services)
        {
            services.AddScoped<IKaikkiDumpFileParser, KaikkiDumpFileParser>();
          
        }
    }
}
