using Shared.Models.Vocabulary;

namespace Logic.Import.Interfaces
{
    public interface IVocabularyValidation
    {
        Task<VocabularyValidationPageModel> GetVocabularyValidationPageModel();
        Task UpdateValidatedVocabularies(List<VocabularyExportModel> vocabularies);
    }
}
