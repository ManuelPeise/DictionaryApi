using Logic.Administration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Vocabulary.Sync;

namespace Service.Api.Controllers.Sync
{
    public class VocabularySyncronizationController : ApiControllerBase
    {
        private readonly IPushSyncronization _vocabularySyncronization;

        public VocabularySyncronizationController(IPushSyncronization vocabularySyncronization)
        {
            _vocabularySyncronization = vocabularySyncronization;
        }

        [HttpPost(Name = "SyncVocabularyDataData")]
        public async Task<VocabularyDataSyncModel> SyncVocabularyDataData([FromBody] VocabularySyncRequestModel requestModel)
        {
            //return await _vocabularySyncronization.SyncData(requestModel);

            return null;
        }
    }
}
