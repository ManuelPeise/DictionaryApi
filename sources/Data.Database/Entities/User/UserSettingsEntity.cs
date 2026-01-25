namespace Data.Database.Entities.User
{
    public class UserSettingsEntity : AEntityBase
    {
        public bool UseLocalDataStore { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
    }
}
