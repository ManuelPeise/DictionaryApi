using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities.User
{
    public class UserEntity : AEntityBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName => EmailAddress;
        public string EmailAddress { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        public DateTime DateOfBirth { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public int UserCredentialsId { get; set; }
        [ForeignKey(nameof(UserCredentialsId))]
        public UserCredentialsEntity? UserCredentials { get; set; }
        public int UserSettingsId { get; set; }
        [ForeignKey(nameof(UserSettingsId))]
        public UserSettingsEntity? UserSettings { get; set; }
    }
}
