namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularyDataSyncModel
    {
        public List<VocabularyLanguageSyncModel> LanguageSyncModels { get; set; } = new List<VocabularyLanguageSyncModel>();
        public List<VocabularyPartOfSpeechSyncModel> PartOfSpeechSyncModels { get; set; } = new List<VocabularyPartOfSpeechSyncModel>();
        public List<VocabularyCategorySyncModel> VocabularyCategorySyncModels { get; set; } = new List<VocabularyCategorySyncModel>();
        public List<VocabularySyncModel> VocabularySyncModels { get; set; } = new List<VocabularySyncModel>();
        public List<VocabularyToCategorySyncModel> VocabularyToCategorySyncModel { get; set; } = new List<VocabularyToCategorySyncModel>();
        public List<VocabularyProgressSyncModel> VocabularyProgressSyncModels { get; set; } = new List<VocabularyProgressSyncModel>();
        public List<VocabularySessionSyncModel> VocabularySessionSyncModels { get; set; } = new List<VocabularySessionSyncModel>();
        public List<VocabularySessionResultSyncModel> VocabulatySessionResultSyncModels { get; set; } = new List<VocabularySessionResultSyncModel>();
    }
}
