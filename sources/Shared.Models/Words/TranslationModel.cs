using Shared.Enums;

namespace Shared.Models.Words
{
    public class TranslationModel
    {
        public string Word { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string PartOfSpeech { get; set; } = string.Empty; 
        public string Sentence { get; set; } = string.Empty;
        public string Ipa { get; set; } = string.Empty;
        public TranslationEnum Language { get; set; }
    }

    public class TranslationJsonModel
    {
        public string Word { get; set; } = string.Empty;
        public string PartOfSpeech { get; set; } = string.Empty;
        public string Ipa { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public TranslationEnum Language { get; set; }
    }
}
