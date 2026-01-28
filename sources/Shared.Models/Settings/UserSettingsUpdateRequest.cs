using Shared.Enums;

namespace Shared.Models.Settings
{
    public class UserSettingsUpdateRequest
    {
        public CultureEnum Culture { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
        public bool UseLocalDataStore { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
