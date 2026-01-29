using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class TranslationEntity : AEntityBase
    {
        public Guid VocabularyGuid { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Ipa { get; set; } = string.Empty;
        public string Sentence { get; set; } = string.Empty;
        public int VocabularyId { get; set; }
        [ForeignKey(nameof(VocabularyId))]
        public VocabularyEntity Vocabulary { get; set; }
        public int LanguageId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public LanguageEntity Language { get; set; }
        // comma separated string
        public string Synonyms { get; set; } = string.Empty;
    }
}
