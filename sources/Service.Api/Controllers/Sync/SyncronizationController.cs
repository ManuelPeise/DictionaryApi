using Logic.Administration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.User.Sync;
using Shared.Models.Vocabulary.Sync;

namespace Service.Api.Controllers.Sync
{
    public class SyncronizationController : ApiControllerBase
    {
        private readonly IPushSyncronization _pushSyncronization;
        private readonly IPullSyncronization _pullSyncronization;

        public SyncronizationController(IPushSyncronization pushSyncronization, IPullSyncronization pullSyncronization)
        {
            _pushSyncronization = pushSyncronization;
            _pullSyncronization = pullSyncronization;
        }


        [ApiAuthentication]
        [HttpGet(Name = "PullUserData")]
        public async Task<UserDataSyncModel?> PullUserData()
        {
            return await _pullSyncronization.PullUserData();
        }

        [ApiAuthentication]
        [HttpGet(Name = "PullVocabularyLanguageSyncModels")]
        public async Task<List<VocabularyLanguageSyncModel>?> PullVocabularyLanguageSyncModels()
        {
            return await _pullSyncronization.PullVocabularyLanguageSyncModels();
        }

        [ApiAuthentication]
        [HttpGet(Name = "PullPartOfSpeechSyncModels")]
        public async Task<List<VocabularyPartOfSpeechSyncModel>?> PullPartOfSpeechSyncModels()
        {
            return await _pullSyncronization.PullPartOfSpeechSyncModels();
        }

        [ApiAuthentication]
        [HttpGet(Name = "PullVocabularyCategorySyncModels")]
        public async Task<List<VocabularyCategorySyncModel>?> PullVocabularyCategorySyncModels()
        {
            return await _pullSyncronization.PullVocabelCategorySyncModels();
        }

        [ApiAuthentication]
        [HttpGet(Name = "PullVocabularyToCategorySyncModels")]
        public async Task<List<VocabularyToCategorySyncModel>?> PullVocabularyToCategorySyncModels([FromBody] List<Guid> externalsIds)
        {
            return await _pullSyncronization.PullVocabularyToCategorySyncModels(externalsIds);
        }

        [HttpPost(Name = "SyncUserData")]
        public async Task<UserDataSyncModel?> SyncUserData([FromBody] UserDataSyncModel requestModel)
        {
            return await _pushSyncronization.SyncUserData(requestModel);
        }

        [ApiAuthentication]
        [HttpPost(Name = "SyncVocabularyProgress")]
        public async Task<List<VocabularyProgressSyncModel>?> SyncVocabularyProgress([FromBody] List<VocabularyProgressSyncModel> requestModel)
        {
            return await _pushSyncronization.SyncVocabularyProgress(requestModel);
        }

        [ApiAuthentication]
        [HttpPost(Name = "SyncVocabularySessions")]
        public async Task<List<VocabularySessionSyncModel>?> SyncVocabularySessions([FromBody] List<VocabularySessionSyncModel> requestModel)
        {
            return await _pushSyncronization.SyncVocabularySessions(requestModel);
        }

        [ApiAuthentication]
        [HttpPost(Name = "SyncVocabularySessionResults")]
        public async Task<List<VocabularySessionResultSyncModel>?> SyncVocabularySessionResults([FromBody] List<VocabularySessionResultSyncModel> requestModel)
        {
            return await _pushSyncronization.SyncVocabularySessionResults(requestModel);
        }

      


    }
}
