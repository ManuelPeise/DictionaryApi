using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Globalization;


namespace Data.Database.Seeds
{
    public class PartOfSpeachSeed : IEntityTypeConfiguration<PartOfSpeechEntity>
    {
        public void Configure(EntityTypeBuilder<PartOfSpeechEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(GetPartOfSpeachSeeds(timeStamp));
        }
         
        private List<PartOfSpeechEntity> GetPartOfSpeachSeeds(DateTime timeStamp)
        {
            var entities = new List<PartOfSpeechEntity>();
            
            var pos = new List<string>
            {
                "adjective",
                "adverb",
                "article",
                "noun",
                "verb",
                "preposition",
                "pronoun",
                "proper noun"
            };

            for (int i = 0; i < pos.Count; i++)
            {
                entities.Add(new PartOfSpeechEntity
                {
                    Id = i + 1,
                    Name = pos[i],
                    ResourceKey = $"PartOfSpeach{pos[i]}",
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                });
            }

            return entities;
        }
    }
}
