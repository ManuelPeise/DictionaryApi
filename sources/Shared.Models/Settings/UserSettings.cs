using Shared.Enums;

namespace Shared.Models.Settings
{
    public class UserSettings
    {
        public CultureEnum Culture { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
        public bool UseLocalDataStore { get; set; }

    }
}
