namespace Shared.Models.Vocabulary
{
    public class VocabularyGroupExportModel
    {
        public Guid VocabularyGroupGuid { get; set; }
        public List<VocabularyExportModel> Vocabularies { get; set; } = new List<VocabularyExportModel>();
    }
}
