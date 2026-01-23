namespace Shared.Models.Authentication
{
    public partial class AuthenticationRequest
    {
        public Guid IdExternal { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
