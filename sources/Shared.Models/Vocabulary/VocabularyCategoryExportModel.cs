namespace Shared.Models.Vocabulary
{
    public class VocabularyCategoryExportModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<VocabularyGroupExportModel> VocabularyGroups { get; set; } = new List<VocabularyGroupExportModel>();
    }
}
