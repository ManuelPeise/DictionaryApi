using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Authentication;
using Shared.Models.Settings;
using Shared.Models.User;

namespace Service.Api.Controllers.User
{
    public class UserProfileController:ApiControllerBase
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [ApiAuthentication]
        [HttpPost(Name = "UpdateProfile")]
        public async Task UpdateProfile([FromBody] UserProfileUpdateRequest updatedUser)
        {
            await _userService.UpdateProfile(updatedUser);
            
        }

        [ApiAuthentication]
        [HttpPost(Name = "UpdatePassword")]
        public async Task UpdatePassword([FromBody] ChangePasswordRequest request)
        {
            await _userService.UpdatePassword(request);

        }

        [ApiAuthentication]
        [HttpPost(Name = "UpdateSettings")]
        public async Task UpdateSettings([FromBody] UserSettingsUpdateRequest updatedSettings)
        {
            await _userService.UpdateUserSettings(updatedSettings);
        }
    }
}
