using Shared.Enums;

namespace Shared.Models.Import
{
    public class VocabularyFileUpload
    {
        public string FileName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public TranslationEnum SourceLanguage { get; set; }
        public List<TranslationEnum> Translations { get; set; } = new List<TranslationEnum>();
        public List<byte> File { get; set; } = new List<byte>();
    }
}
