using Shared.Enums;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyTopicEntity : AEntityBase
    {
        public Guid GroupGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public TranslationEnum SourceLanguage { get; set; }
        public ICollection<VocabularyEntity> Vocabularies { get; set; } = new List<VocabularyEntity>();
    }
}
