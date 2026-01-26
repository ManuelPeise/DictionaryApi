namespace Shared.Models.Authentication
{
    public class ChangePasswordRequest
    {
        public string IdExternal { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string PasswordReplication { get; set; } = string.Empty;
    }
}
