using Shared.Enums;
using Shared.Models.Vocabulary.Sync;


namespace Shared.Models.User.Sync
{
    public class UserDataSyncModel: SyncModelBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        public DateTime DateOfBirth { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public int UserCredentialsId { get; set; }
        public UserCredentialsSyncModel UserCredentials { get; set; } = new UserCredentialsSyncModel();
        public int UserSettingsId { get; set; }
        public UserSettingsSyncModel UserSettings { get; set; } = new UserSettingsSyncModel();
    }
}
