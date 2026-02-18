using Shared.Models.Vocabulary;
using Shared.Models.Vocabulary.Validation;

namespace Logic.Import.Interfaces
{
    public interface IVocabularyValidationModule
    {
        Task<VocabularyValidationModel> GetVocabularyValidationModel();
        Task<List<VocabularyExportModel>> UpdateValidatedVocabularies(List<VocabularyExportModel> vocabularies);
    }
}
