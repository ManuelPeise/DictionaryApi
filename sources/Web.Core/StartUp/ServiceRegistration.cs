using Data.Database;
using Logic.Shared.DI;
using Logic.Words.DI;
using Microsoft.EntityFrameworkCore;

namespace Web.Core.StartUp
{
    internal static class ServiceRegistration
    {
        internal static void RegisterServices(this IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString("DictionaryAppDb");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DictionaryAppDb' not found.");
                }

                options.UseMySQL(connectionString);
            });

            services.RegisterSharedServices();
            services.RegisterWordServices();

            services.AddControllers();
            services.AddOpenApi();
            services.AddSwaggerGen();
        }
    }
}
