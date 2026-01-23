using Shared.Models.Authentication;

namespace Logic.UserService.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationRequest request);
        Task<bool> Logout(string idExternal);
    }
}
