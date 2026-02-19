using Shared.Models.User.Sync;
using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration.Interfaces
{
    public interface IPushSyncronization
    {
        Task<UserDataSyncModel?> SyncUserData(UserDataSyncModel model);
        Task<List<VocabularySessionSyncModel>?> SyncVocabularySessions(List<VocabularySessionSyncModel> models);
        Task<List<VocabularyProgressSyncModel>?> SyncVocabularyProgress(List<VocabularyProgressSyncModel> models);
        Task<List<VocabularySessionResultSyncModel>?> SyncVocabularySessionResults(List<VocabularySessionResultSyncModel> models);
    }
}
