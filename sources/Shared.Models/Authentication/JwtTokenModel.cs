namespace Shared.Models.Authentication
{
    public class JwtTokenModel
    {
        public string SecurityKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
    }
}
