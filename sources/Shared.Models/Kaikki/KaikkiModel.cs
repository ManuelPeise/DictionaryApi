using System.Text.Json.Serialization;

namespace Shared.Models.Kaikki
{
    public sealed class KaikkiModel
    {
        [JsonPropertyName("pos")]
        public string? PartOfSpeech { get; set; }

        [JsonPropertyName("word")]
        public string? Word { get; set; }

        [JsonPropertyName("lang_code")]
        public string? LanguageCode { get; set; }
        [JsonPropertyName("lang")]
        public string? LanguageName { get; set; }

        [JsonPropertyName("translations")]
        public List<KaikkiTranslation>? Translations { get; set; }

        [JsonPropertyName("senses")]
        public List<KaikkiSenses>? Senses { get; set; }
        [JsonPropertyName("sounds")]
        public List<KaikkiSound>? Sounds { get; set; }
    }

    public sealed class KaikkiSenses
    {
        [JsonPropertyName("synonyms")]
        public List<KaikkiSynonym>? Synonyms { get; set; }
    }

    public sealed class KaikkiSynonym
    {
        [JsonPropertyName("word")]
        public string Word { get; set; } = string.Empty;
    }
    public sealed class KaikkiTranslation
    {
        [JsonPropertyName("lang")]
        public string? LanguageName { get; set; }
        [JsonPropertyName("lang_code")]
        public string? LanguageCode { get; set; }
        [JsonPropertyName("sense")]
        public string? Sentence { get; set; }
        [JsonPropertyName("word")]
        public string? Word { get; set; }
    }

    public sealed class KaikkiSound
    {
        [JsonPropertyName("ipa")]
        public string? Ipa { get; set; }
    }

}
