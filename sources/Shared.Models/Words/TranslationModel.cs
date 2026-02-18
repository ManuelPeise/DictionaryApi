using Shared.Enums;

namespace Shared.Models.Words
{
    public class TranslationModel
    {
        public string Word { get; set; } = string.Empty;
        public string? Article { get; set; }
        public string PartOfSpeech { get; set; } = string.Empty; 
        public string? Sentence { get; set; }
        public string? Ipa { get; set; }
        public TranslationEnum Language { get; set; }
    }
}
