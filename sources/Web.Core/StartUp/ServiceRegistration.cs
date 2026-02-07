using Data.Accessor.DI;
using Data.Database;
using Logic.Import.DI;
using Logic.Shared.DI;
using Logic.UserService.DI;
using Logic.Words.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Quartz;
using Quartz.Simpl;
using Shared.Models;
using Shared.Models.Authentication;
using Shared.Models.Settings;
using System.Text;
using System.Threading.RateLimiting;

namespace Web.Core.StartUp
{
    internal static class ServiceRegistration
    {
        internal static void RegisterServices(this IServiceCollection services, IConfiguration configuration, string corsePolicy)
        {
            services.Configure<JwtTokenModel>(configuration.GetSection("Jwt"));
            services.Configure<UserSettings>(configuration.GetSection("Settings"));
            services.Configure<FileSystemConfiguration>(configuration.GetSection("FileSystemConfiguration"));
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixedRateLimit", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5; 
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 2;
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter", _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(corsePolicy, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddQuartz(q =>
            {
                q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
            services.RegisterImportServices();

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
