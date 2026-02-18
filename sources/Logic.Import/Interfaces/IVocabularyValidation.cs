using Shared.Models.Vocabulary;

namespace Logic.Import.Interfaces
{
    public interface IVocabularyValidation
    {
        Task<VocabularyValidationPageModel> GetVocabularyValidationPageModel();
        Task<VocabularyUpdateResponse?> UpdateValidatedVocabularies(List<VocabularyExportModel> vocabularies);
    }
}
