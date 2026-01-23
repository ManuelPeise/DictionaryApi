using Shared.Enums;

namespace Shared.Models.Authentication
{
    public partial class AuthenticationResult
    {
        public bool Result { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefeshToken { get; set; } = string.Empty;

    }
}
