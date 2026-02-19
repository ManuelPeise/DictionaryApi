namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularySyncRequestModel
    {
        public List<Guid> CategoryGuids { get; set; } = new List<Guid>();
        public List<VocabularyProgressSyncModel> VocabularyProgressSyncModels { get; set; } = new List<VocabularyProgressSyncModel>();
        public List<VocabularySessionSyncModel> VocabularySessionSyncModels { get; set; } = new List<VocabularySessionSyncModel>();
        public List<VocabularySessionResultSyncModel> VocabulatySessionResultSyncModels { get; set; } = new List<VocabularySessionResultSyncModel>();
    }
}
