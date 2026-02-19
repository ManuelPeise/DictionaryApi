using Shared.Enums;

namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularyCategorySyncModel: SyncModelBase
    {
        public Guid GroupGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public TranslationEnum SourceLanguage { get; set; }
    }
}
