using Shared.Models.Vocabulary.Sync;

namespace Logic.Administration.Interfaces
{
    public interface IVocabularySyncronization
    {
        Task<VocabularyDataSyncModel> SyncData(VocabularySyncRequestModel requestModel);
    }
}
