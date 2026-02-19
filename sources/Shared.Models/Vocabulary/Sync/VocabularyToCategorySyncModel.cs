namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularyToCategorySyncModel: SyncModelBase
    {
        public int VocabularyId { get; set; }
        public int CategoryId { get; set; }
        public VocabularySyncModel Vocabulary { get; set; } = new VocabularySyncModel();

    }
}
