using Shared.Models.Import;

namespace Logic.Import.Interfaces
{
    public interface IVocabularyImporter
    {
        Task<bool> ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel);
    }
}
