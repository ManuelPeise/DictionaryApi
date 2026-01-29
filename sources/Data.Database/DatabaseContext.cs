using Data.Database.Entities;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Data.Database.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Data.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserSeed());
            modelBuilder.ApplyConfiguration(new UserCredentialsSeed());
            modelBuilder.ApplyConfiguration(new UserSettingsSeed());
            modelBuilder.ApplyConfiguration(new LanguageSeed());
            modelBuilder.ApplyConfiguration(new PartOfSpeachSeed());
        }

        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }

        // vocabulary tabels
        public DbSet<LanguageEntity> LanguageTable { get; set; }
        public DbSet<PartOfSpeachEntity> PartOfSpeachTable { get; set; }
        public DbSet<VocabularyEntity> VocabularyTable { get; set; }
        public DbSet<TranslationEntity> TranslationTable { get; set; }

    }
}
