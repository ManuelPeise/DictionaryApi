using Shared.Models.Import;

namespace Logic.Import.Interfaces
{
    public interface IVocabularyImporter
    {
        Task ImportVocabularyFilesAsync();
        Task<bool> ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel);
    }
}
