using Data.Database.Entities;
using Data.Database.Entities.User;
using Data.Database.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Models.Settings;

namespace Data.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IOptions<UserSettings> _apiSettings;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IOptions<UserSettings> apiSettings) : base(options)
        {
            _apiSettings = apiSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserSeed());
            modelBuilder.ApplyConfiguration(new UserCredentialsSeed());
            modelBuilder.ApplyConfiguration(new UserSettingsSeed());
        }

        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }
     

    }
}
