using Shared.Enums;
using System.Text.Json.Serialization;

namespace Logic.Parsing.Models
{
    public class KaikkiModel
    {
        [JsonPropertyName("pos")]
        public string? PartOfSpeech { get; set; }
        [JsonPropertyName("word")]
        public string Word { get; set; } = string.Empty;
        public string NormalizedWord => Word.Normalize();
        [JsonPropertyName("lang")]
        public string? Language { get; set; }
        [JsonPropertyName("lang_code")]
        public string? LanguageCode { get; set; }
        [JsonPropertyName("related")]
        public List<KaikkiRelations> Relations { get; set; } = new  List<KaikkiRelations>();
        [JsonPropertyName("sounds")]
        public List<KaikkiSounds> Sounds { get; set; } = new List<KaikkiSounds>();
        [JsonPropertyName("forms")]
        public List<KaikkiForm> Forms { get; set; } = new List<KaikkiForm>();
        [JsonPropertyName("translations")]
        public List<KaikkiTranslation> Translations { get; set; } = new List<KaikkiTranslation>();
    }

    public class KaikkiRelations
    {
        [JsonPropertyName("word")]
        public string? Word { get; set; }
    }

    public class KaikkiSounds
    {
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();
        [JsonPropertyName("ipa")]
        public string? Ipa { get; set; }
    }

    public class KaikkiForm
    {
        public string? Form { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class KaikkiTranslation
    {
        [JsonPropertyName("word")]
        public string Word { get; set; } = string.Empty;
        public string NormalizedWord => Word.Normalize();
        [JsonPropertyName("lang")]
        public string? Language { get; set; }
        [JsonPropertyName("code")]
        public string? LanguageCode { get; set; }

    }

    public class KaikkiKey
    {
        public string Word { get; set; } = string.Empty;
        public TranslationEnum Language { get; set; }

    }
}
