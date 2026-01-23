namespace Shared.Models.KaikkiJsonModels
{
    public class WordTranslationModel
    {
        public string? Word { get; set; }
        public string? Ipa { get; set; }
        public string? Sentence { get; set; }
        public string? LanguageCode { get; set; }
        public string? LanguageName { get; set; }
        public List<string>? Synonyms { get; set; }
    }
}
