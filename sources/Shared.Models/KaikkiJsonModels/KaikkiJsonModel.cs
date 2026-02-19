using Shared.Enums;

namespace Shared.Models.KaikkiJsonModels
{
    public class KaikkiJsonDataModel
    {
        public string? PartOfSpeech { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? LanguageName { get; set; }
        public string? LanguageCode { get; set; }
        public string? Ipa { get; set; }
        public List<WordTranslationModel>? Translations { get; set; }
        public List<string>? Synonyms { get; set; }

        public KaikkiJsonDataExtract ToDataExtract()
        {
            return new KaikkiJsonDataExtract
            {
                PartOfSpeech = this.PartOfSpeech,
                Word = this.Word,
                Ipa = this.Ipa,
                LanguageCode = this.LanguageCode,
                Synonyms = this.Synonyms
            };
        }
    }

    public class KaikkiJsonDataExtract
    {
        public string? PartOfSpeech { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? Ipa { get; set; }
        public string? LanguageCode { get; set; }
        public List<string>? Synonyms { get; set; }
    }
}
