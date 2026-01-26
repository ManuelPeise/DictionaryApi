using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.UserService.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationRequest request);
        Task<bool> Logout(string idExternal);
        Task<UserModel?> GetCurrentUserData();
        Task UpdateProfile(UserProfileUpdateRequest updatedUser);
        Task UpdatePassword(ChangePasswordRequest request);
    }
}
