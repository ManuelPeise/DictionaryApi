using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
using System.Globalization;

namespace Data.Database.Seeds
{
    internal class LanguageSeed : IEntityTypeConfiguration<LanguageEntity>
    {
        public void Configure(EntityTypeBuilder<LanguageEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(new List<LanguageEntity> {
            
                new LanguageEntity
                {
                    Id = 1,
                    Name = "German",
                    ResourceKey = "LanguageGerman",
                    TranslationType = TranslationEnum.De,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                },
                new LanguageEntity
                {
                    Id = 2,
                    Name = "English",
                    ResourceKey = "LanguageEnglish",
                    TranslationType = TranslationEnum.En,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                },
                new LanguageEntity
                {
                    Id = 3,
                    Name = "Danish",
                    ResourceKey = "LanguageDanish",
                    TranslationType = TranslationEnum.Da,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                }
            });
        }
    }
}
