using Logic.Import.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Import;

namespace Service.Api.Controllers.Vovabulary
{
    public class VocabularyImportController : ApiControllerBase
    {
        private readonly IFileImporter _fileImporter;
        private readonly IVocabularyImporter _vocabularyImporter;

        public VocabularyImportController(IFileImporter fileImporter, IVocabularyImporter vocabularyImporter)
        {
            _fileImporter = fileImporter;
            _vocabularyImporter = vocabularyImporter;
        }

        [HttpPost(Name = "ImportVocabulary")]
        public async Task ImportVocabulary([FromBody] VocabularyFileUpload fileUpload)
        {
           await _fileImporter.ImportVocabularyFileAsync(fileUpload);
        }

        
        [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecuteVocabularyTask")]
        public async Task ExecuteVocabularyTask()
        {
           await _vocabularyImporter.ImportVocabularyFilesAsync();
        }
    }
}
