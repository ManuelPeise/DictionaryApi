using Shared.Enums;

namespace Shared.Models.Import
{
    public class VocabularyImportWordModel
    {
        public string Word { get; set; } = string.Empty;
        public string? Article { get; set; }
        public string PartOfSpeech { get; set; } = string.Empty;
        public TranslationEnum Language { get; set; }
        public string? Ipa { get; set; }
        public string ExampleSentence { get; set; } = string.Empty;
        public List<VocabularyImportWordModel> Translations { get; set; } = new List<VocabularyImportWordModel>();
    }
}
