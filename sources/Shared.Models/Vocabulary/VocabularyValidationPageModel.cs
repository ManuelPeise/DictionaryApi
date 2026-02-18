namespace Shared.Models.Vocabulary
{
    public class VocabularyValidationPageModel
    {
        public List<VocabularyCategoryExportModel> Categories { get; set; } = new List<VocabularyCategoryExportModel>();
        public List<VocabularyCategoryDropdownModel> CategoryDropDownItems { get; set; } = new List<VocabularyCategoryDropdownModel>();
    }

    public class VocabularyCategoryDropdownModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
