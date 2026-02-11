using Shared.Enums;

namespace Data.Database.Entities.Files
{
    public class ImportFileEntity : AEntityBase
    {
        public string FileName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public TranslationEnum SourceLanguage { get; set; }
        public List<TranslationEnum> Translations { get; set; }
        public FileImportStatus Status { get; set; }
        public List<byte> FileBytes { get; set; } = new List<byte>();
        public bool IsImportedSuccessful { get; set; }
    }
}
