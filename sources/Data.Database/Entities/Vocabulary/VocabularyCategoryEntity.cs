using Shared.Enums;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyCategoryEntity:AEntityBase
    {
        public Guid GroupGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public TranslationEnum SourceLanguage { get; set; }
        public ICollection<VocabularyToCategoryEntity> VocabulariesToCategoryEntities { get; set; } = new List<VocabularyToCategoryEntity>();
    }
}
