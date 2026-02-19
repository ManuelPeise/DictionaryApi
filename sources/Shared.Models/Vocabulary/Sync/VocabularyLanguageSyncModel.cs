using Shared.Enums;

namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularyLanguageSyncModel: SyncModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string ResourceKey { get; set; } = string.Empty;
        public TranslationEnum TranslationType { get; set; }
    }
}
