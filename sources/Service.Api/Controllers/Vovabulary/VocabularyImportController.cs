using Logic.Import.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Import;

namespace Service.Api.Controllers.Vovabulary
{
    public class VocabularyImportController : ApiControllerBase
    {
        private readonly IVocabularyImporter _vocabularyImporter;

        public VocabularyImportController(IVocabularyImporter vocabularyImporter)
        {
            _vocabularyImporter = vocabularyImporter;
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "ImportVocabularyFile")]
        public async Task<bool> ImportVocabularyFile([FromBody] VocabularyFileUpload fileUpload)
        {
            return await _vocabularyImporter.ImportVocabularyFileAsync(fileUpload);
        }
    }
}
