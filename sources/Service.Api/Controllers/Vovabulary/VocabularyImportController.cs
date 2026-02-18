using Logic.Import.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Import;
using Shared.Models.Vocabulary;

namespace Service.Api.Controllers.Vovabulary
{
    public class VocabularyImportController : ApiControllerBase
    {
        private readonly IFileImporter _fileImporter;
        private readonly IVocabularyImporter _vocabularyImporter;
        private readonly IVocabularyValidation _vocabularyValidation;

        public VocabularyImportController(IFileImporter fileImporter, IVocabularyImporter vocabularyImporter, IVocabularyValidation vocabularyValidation)
        {
            _fileImporter = fileImporter;
            _vocabularyImporter = vocabularyImporter;
            _vocabularyValidation = vocabularyValidation;
        }


        [HttpGet(Name = "GetVocabularyValidationPageModel")]
        public async Task<VocabularyValidationPageModel> GetVocabularyValidationPageModel()
        {
            return await _vocabularyValidation.GetVocabularyValidationPageModel();
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "ImportVocabularyFile")]
        public async Task<bool> ImportVocabularyFile([FromBody] VocabularyFileUpload fileUpload)
        {
            return await _vocabularyImporter.ImportVocabularyFileAsync(fileUpload);
        }

        [ApiAuthentication(RequiredRole = UserRoleEnum.Admin)]
        [HttpPost(Name = "UpdateVocabularies")]
        public async Task<VocabularyUpdateResponse?> UpdateVocabularies([FromBody] List<VocabularyExportModel> vocabularies)
        {
            return await _vocabularyValidation.UpdateValidatedVocabularies(vocabularies);
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecuteVocabularyTask")]
        public async Task ExecuteVocabularyTask()
        {
            await _vocabularyImporter.ImportVocabularyFilesAsync();
        }
    }
}
