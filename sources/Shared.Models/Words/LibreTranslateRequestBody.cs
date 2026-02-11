using System.Text.Json.Serialization;

namespace Shared.Models.Words
{
    public class LibreTranslateRequestBody
    {
        [JsonPropertyName("q")]
        public string? Word { get; set; }
        [JsonPropertyName("source")]
        public string? SourceLanguage { get; set; }
        [JsonPropertyName("target")]
        public string? TargetLanguage { get; set; }
        [JsonPropertyName("format")]
        public string? Format { get; set; } = "text";
    }
}
