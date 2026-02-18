namespace Shared.Models.Vocabulary.Validation
{
    public class VocabularyValidationModel
    {
        public List<DropdownModel> CategoryDropdownItems { get; set; } = new List<DropdownModel>();
        public List<VocabularyExportModel> Vocabularies { get; set; } = new List<VocabularyExportModel>();
    }
}
