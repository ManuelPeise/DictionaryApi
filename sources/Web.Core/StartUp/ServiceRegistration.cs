using Data.Accessor.DI;
using Data.Database;
using Logic.Shared.DI;
using Logic.UserService.DI;
using Logic.Words.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shared.Models.Authentication;
using Shared.Models.Settings;
using System.Text;

namespace Web.Core.StartUp
{
    internal static class ServiceRegistration
    {
        internal static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenModel>(configuration.GetSection("Jwt"));
            services.Configure<UserSettings>(configuration.GetSection("Settings"));

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

            services.AddHttpContextAccessor();
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

            RegisterSwagger(services);

            services.AddControllers();

        }

        private static void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Dictionary API",
                    Version = "v1",
                    Description = "API für Wörterbuch-Anwendung",
                    Contact = new OpenApiContact
                    {
                        Name = "Manuel Peise",
                        Email = "manuel.p80@gmx.de"
                    }
                });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
                });
            });
        }
    }
}
