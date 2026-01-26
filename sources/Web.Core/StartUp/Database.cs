using Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.Core.StartUp
{
    internal static class Database
    {
        internal static void Migrate(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseContext>>();

                try
                {
                    if (db.Database.GetPendingMigrations().Any())
                    {
                        logger.LogInformation("Applying pending migrations...");
                        db.Database.Migrate();
                        logger.LogInformation("Migrations applied successfully.");
                    }
                    else
                    {
                        logger.LogInformation("No pending migrations.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database. Continuing with application startup...");
                    // Don't crash the application if migrations fail
                    // This allows the app to start even if there are migration issues
                }
            }
        }
    }
}
