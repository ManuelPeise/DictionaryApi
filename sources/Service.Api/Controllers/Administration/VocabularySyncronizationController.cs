using Logic.Administration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Vocabulary.Sync;

namespace Service.Api.Controllers.Administration
{
    public class VocabularySyncronizationController : ApiControllerBase
    {
        private readonly IVocabularySyncronization _vocabularySyncronization;

        public VocabularySyncronizationController(IVocabularySyncronization vocabularySyncronization)
        {
            _vocabularySyncronization = vocabularySyncronization;
        }

        [HttpPost(Name = "SyncVocabularyDataData")]
        public async Task<VocabularyDataSyncModel> SyncVocabularyDataData([FromBody] VocabularySyncRequestModel requestModel)
        {
            return await _vocabularySyncronization.SyncData(requestModel);
        }
    }
}
