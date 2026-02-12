using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.Vocabulary
{
    public class VocabularyEntity : AEntityBase
    {
        public Guid GroupGuid { get; set; }
        public string Word { get; set; } = string.Empty;
        public string? Article { get; set; }
        public string? ExampleSentence { get; set; } = string.Empty;
        public string? Ipa { get; set; }
        public bool IsReviewRequired { get; set; }
        public int LanguageId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public LanguageEntity Language { get; set; }
        public int PartOfSpeechId { get; set; }
        [ForeignKey(nameof(PartOfSpeechId))]
        public PartOfSpeechEntity PartOfSpeech { get; set; }
        public ICollection<VocabularyToCategoryEntity> VocabulatyToCategoryEntities { get; set; } = new List<VocabularyToCategoryEntity>();
    }
}
