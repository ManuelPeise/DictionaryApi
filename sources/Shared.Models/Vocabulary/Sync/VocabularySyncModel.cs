namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularySyncModel: SyncModelBase
    {
        public Guid GroupGuid { get; set; }
        public string Word { get; set; } = string.Empty;
        public string? Article { get; set; }
        public string? ExampleSentence { get; set; } = string.Empty;
        public string? Ipa { get; set; }
        public bool IsReviewRequired { get; set; }
        public int LanguageId { get; set; }
        public int PartOfSpeechId { get; set; }
    }
}
