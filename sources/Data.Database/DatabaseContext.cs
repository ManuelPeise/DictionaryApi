using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Data.Database.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Models.Settings;

namespace Data.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly ApiSettings _apiSettings;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IOptions<ApiSettings> apiSettings) : base(options)
        {
            _apiSettings = apiSettings.Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserSeed());
            modelBuilder.ApplyConfiguration(new UserCredentialsSeed());
            modelBuilder.ApplyConfiguration(new UserSettingsSeed());
            modelBuilder.ApplyConfiguration(new LanguageSeed());
            modelBuilder.ApplyConfiguration(new PartOfSpeachSeed());
            modelBuilder.ApplyConfiguration(new ScheduledTaskSeed(_apiSettings));
        }

        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }

        // vocabulary tabels
        public DbSet<LanguageEntity> LanguageTable { get; set; }
        public DbSet<PartOfSpeechEntity> PartOfSpeachTable { get; set; }
        public DbSet<VocabularyEntity> VocabularyTable { get; set; }
        public DbSet<VocabularyTopicEntity> VocabularyTopicTable { get; set; }

        // files
        public DbSet<ImportFileEntity> ImportFileTable { get; set; }

        // tasks
        public DbSet<ScheduledTaskEntity> ScheduledTaskTable { get; set; }  

    }
}
