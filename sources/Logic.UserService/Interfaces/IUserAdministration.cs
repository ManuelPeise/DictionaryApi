using Shared.Models.Administration;

namespace Logic.UserService.Interfaces
{
    public interface IUserAdministration
    {
        Task<UserRegistrationResult?> RegisterUser(UserRegistrationRequestModel requestModel);
    }
}
