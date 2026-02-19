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
                    IdExternal = new Guid("18fda2d3-3f6f-4963-8495-d498defbb54d"),
                    Name = "German",
                    ResourceKey = "LanguageGerman",
                    TranslationType = TranslationEnum.De,
                    IsDirty = false,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                },
                new LanguageEntity
                {
                    Id = 2,
                    IdExternal = new Guid("16dd7950-90d0-4e80-a80b-a4268e65d736"),
                    Name = "English",
                    ResourceKey = "LanguageEnglish",
                    TranslationType = TranslationEnum.En,
                    IsDirty = false,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                },
                new LanguageEntity
                {
                    Id = 3,
                    IdExternal = new Guid("65ae65cb-3dff-4d98-9a62-42b2cd8c6140"),
                    Name = "Danish",
                    ResourceKey = "LanguageDanish",
                    TranslationType = TranslationEnum.Da,
                    IsDirty = false,
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                }
            });
        }
    }
}
