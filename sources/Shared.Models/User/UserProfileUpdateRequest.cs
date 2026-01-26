namespace Shared.Models.User
{
    public class UserProfileUpdateRequest
    {
        public int Id { get; set; }
        public string UserIdExternal { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        public DateTime DateOfBirth { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
