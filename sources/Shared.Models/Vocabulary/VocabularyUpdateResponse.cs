namespace Shared.Models.Vocabulary
{
    public class VocabularyUpdateResponse
    {
        public string GroupGuid { get; set; }
        public List<VocabularyExportModel> Vocabularies { get; set; }
    }
}
