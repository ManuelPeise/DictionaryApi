namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularyProgressSyncModel: SyncModelBase
    {
        public int UserId { get; set; }
        public int TimesSeen { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int VocabularyId { get; set; }
    }
}
