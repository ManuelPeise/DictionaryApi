using Shared.Enums;
using Shared.Models.Settings;
using System.ComponentModel.DataAnnotations.Schema;


namespace Shared.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public Guid IdExternal { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        public DateTime DateOfBirth { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public int UserCredentialsId { get; set; }
        [ForeignKey(nameof(UserCredentialsId))]
        public UserCredentials? UserCredentials { get; set; }
        public int UserSettingsId { get; set; }
        [ForeignKey(nameof(UserSettingsId))]
        public UserSettings? UserSettings { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
