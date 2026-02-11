using System.Text.Json.Serialization;

namespace Shared.Models.Words
{
    public class LibreTranslation
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
    }
}