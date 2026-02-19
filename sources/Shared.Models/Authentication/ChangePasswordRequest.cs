namespace Shared.Models.Authentication
{
    public class ChangePasswordRequest
    {
        public Guid IdExternal { get; set; } 
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string PasswordReplication { get; set; } = string.Empty;
    }
}
