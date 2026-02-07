using Shared.Models.Import;

namespace Logic.Import.Interfaces
{
    public interface IFileImporter
    {
        Task ImportVocabularyFileAsync(VocabularyFileUpload fileUploadModel);
    }
}
