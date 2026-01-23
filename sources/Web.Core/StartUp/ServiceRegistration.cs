using Data.Accessor.DI;
using Data.Database;
using Logic.Shared.DI;
using Logic.UserService.DI;
using Logic.Words.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.Authentication;
using System.Text;

namespace Web.Core.StartUp
{
    internal static class ServiceRegistration
    {
        internal static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenModel>(configuration.GetSection("Jwt"));
            
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

            var jwtConfig = configuration.GetSection("Jwt").Get<JwtTokenModel>();

            if (jwtConfig == null)
            {
                throw new InvalidOperationException("JWT configuration section is missing or invalid.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var key = jwtConfig?.SecurityKey ?? string.Empty;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig?.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.RegisterSharedServices();
            services.RegisterWordServices();
            services.RegisterDataAccessorServices();
            services.RegisterUserServices();

            services.AddControllers();
            services.AddOpenApi();
            services.AddSwaggerGen();
        }
    }
}
