
using Shared.Models.Vocabulary.Sync;

namespace Shared.Models.User.Sync
{
    public class UserCredentialsSyncModel: SyncModelBase
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
