using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Data.Database.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Models.Settings;
using System.Collections.Generic;

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

            modelBuilder.Entity<VocabularyToCategoryEntity>(entity =>
            {
                entity.HasIndex(e => new { e.VocabularyId, e.CategoryId }).IsUnique();

                entity.HasOne(e => e.Vocabulary)
                    .WithMany(v => v.VocabulatyToCategoryEntities)
                    .HasForeignKey(e => e.VocabularyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.VocabulariesToCategoryEntities)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VocabularySessionEntity>(entity =>
            {
                entity.HasMany(e => e.Vocabularies)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "VocabularySessionVocabulary",
                        j => j.HasOne<VocabularyEntity>()
                            .WithMany()
                            .HasForeignKey("VocabularyId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<VocabularySessionEntity>()
                            .WithMany()
                            .HasForeignKey("SessionId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("SessionId", "VocabularyId");
                        });
            });
        }

        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }

        // vocabulary tabels
        public DbSet<LanguageEntity> LanguageTable { get; set; }
        public DbSet<PartOfSpeechEntity> PartOfSpeachTable { get; set; }
        public DbSet<VocabularyEntity> VocabularyTable { get; set; }
        public DbSet<VocabularyCategoryEntity> VocabularyCategoryTable { get; set; }
        public DbSet<VocabularyToCategoryEntity> VocabularyToCategoriesTable { get; set; }
        public DbSet<VocabularySessionEntity> VocabularySessionTable { get; set; }
        public DbSet<VocabularySessionResultEntity> VocabularySessionResultTable { get; set; }
        public DbSet<VocabularyProgressEntity> VocabularyProgressTable { get; set; }

        // files
        public DbSet<ImportFileEntity> ImportFileTable { get; set; }

        // tasks
        public DbSet<ScheduledTaskEntity> ScheduledTaskTable { get; set; }  

    }
}
