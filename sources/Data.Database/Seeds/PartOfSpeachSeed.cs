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
            
            var pos = new Dictionary<string, string>
            {
                { "adjective", "e70f0216-1360-41af-9e4c-40f06574deb4" },
                { "adverb", "132a81ea-c686-498e-b593-d6f871ffd6ea" },
                { "article", "00783292-d4f8-439e-bb2a-8ced803f0e70" },
                { "noun", "3fd7b958-22fc-4389-9687-599b9d05d6d3" },
                { "verb", "3548d4e7-e5fe-4280-9362-40b6016569b4" },
                { "preposition", "06ebc0cd-7d23-4958-bb28-2b936f079359" },
                { "pronoun", "ebc23f3f-7e7c-4dec-a50d-6a32917e9b48" },
                { "proper noun", "7aac8e85-19ed-462e-8ef5-5a916c960133" }
            };

            int id = 1;

            foreach (var key in pos.Keys)
            {
                entities.Add(new PartOfSpeechEntity
                {
                    Id = id,
                    IdExternal = new Guid(pos[key]),
                    Name = key,
                    ResourceKey = $"PartOfSpeach{pos[key]}",
                    CreatedBy = "System",
                    CreatedAt = timeStamp,
                });
                id++;
            }
            
            return entities;
        }
    }
}
