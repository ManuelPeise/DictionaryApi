using Shared.Enums;

namespace Data.Database.Entities.Vocabulary
{
    public class LanguageEntity : AEntityBase
    {
        public string Name { get; set; } = string.Empty;
        public string ResourceKey { get; set; } = string.Empty;
        public LanguageEnum LanguageType { get; set; }
        public ICollection<VocabularyEntity> Vocabularies { get; set; } = new List<VocabularyEntity>();
    }
}
