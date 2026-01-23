namespace Shared.Models.Words
{
    public class WordModel
    {
        public int Id { get; set; }
        public Guid GroupId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string? Ipa { get; set; }
    }
}
