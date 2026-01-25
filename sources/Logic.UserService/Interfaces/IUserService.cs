using Logic.Shared.Models;
using Shared.Models.Authentication;

namespace Logic.UserService.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationRequest request);
        Task<bool> Logout(string idExternal);
        Task<CurrentUser?> GetCurrentUserData();
        Task<CurrentUser?> UpdateUserData(CurrentUser updatedUser);
    }
}
