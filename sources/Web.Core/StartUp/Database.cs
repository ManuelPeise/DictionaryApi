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

                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
        }
    }
}
