using Shared.Enums;
using Shared.Models.Vocabulary.Sync;

namespace Shared.Models.Settings
{
    public class UserSettings: SyncModelBase
    {
        public CultureEnum Culture { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
        public bool UseLocalDataStore { get; set; }

    }
}
