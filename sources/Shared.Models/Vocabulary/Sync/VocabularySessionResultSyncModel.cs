namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularySessionResultSyncModel:SyncModelBase
    {
        public int UserId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int SessionId { get; set; }
    }
}
