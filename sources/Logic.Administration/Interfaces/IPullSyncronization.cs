using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration.Interfaces
{
    public interface IPullSyncronization
    {
        Task<List<VocabularyLanguageSyncModel>> PullVocabularyLanguageSyncModels();
        Task<List<VocabularyPartOfSpeechSyncModel>> PullPartOfSpeechSyncModels();
        Task<List<VocabularyCategorySyncModel>> PullVocabelCategorySyncModels();
        Task<List<VocabularyToCategorySyncModel>> PullVocabularyToCategorySyncModels(List<Guid> categoryExternalGuids);
    }
}
