using Shared.Enums;

namespace Data.Database.Entities.User
{
    public class UserSettingsEntity : AEntityBase
    {
        public CultureEnum Culture { get; set; }
        public bool UseLocalDataStore { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
    }
}
