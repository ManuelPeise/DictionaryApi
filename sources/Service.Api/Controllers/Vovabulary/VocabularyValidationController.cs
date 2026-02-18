using Logic.Import.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Vocabulary;
using Shared.Models.Vocabulary.Validation;

namespace Service.Api.Controllers.Vovabulary
{
    public class VocabularyValidationController: ApiControllerBase
    {
        private readonly IVocabularyValidationModule _vocabularyValidationModule;

        public VocabularyValidationController(IVocabularyValidationModule vocabularyValidationModule)
        {
            _vocabularyValidationModule = vocabularyValidationModule;
        }

        [HttpGet(Name = "GetInitialData")]
        public async Task<VocabularyValidationModel> GetInitialData()
        {
            return await _vocabularyValidationModule.GetVocabularyValidationModel();
        }

        [HttpPost(Name = "UpdateValidatedVocabularies")]
        public async Task<List<VocabularyExportModel>> UpdateValidatedVocabularies([FromBody] List<VocabularyExportModel> vocabularies)
        {
            return await _vocabularyValidationModule.UpdateValidatedVocabularies(vocabularies);
        }
    }
}
