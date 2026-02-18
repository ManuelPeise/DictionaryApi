using Shared.Enums;

namespace Shared.Models.Vocabulary
{
    public class VocabularyExportModel
    {
        public int Id { get; set; }
        public Guid VocabularyGroupGuid { get; set; }
        public int VocabularyCategoryId { get; set; }
        public string VocabularyCategoryName { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? Article { get; set; }
        public string PartOfSpeech { get; set; } = string.Empty;
        public string? Sentence { get; set; }
        public string? Ipa { get; set; }
        public TranslationEnum Language { get; set; }
        public bool IsValidated { get; set; }
        public string? LastUpdatedAtBy { get; set; }
    }
}
