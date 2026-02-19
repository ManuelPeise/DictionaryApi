using Shared.Enums;

namespace Shared.Models.Vocabulary.Sync
{
    public class VocabularySessionSyncModel: SyncModelBase
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public VocabularySessionType SessionType { get; set; }
    }
}
