using Logic.Shared.Models;
using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Authentication;

namespace Service.Api.Controllers.UserAdministration
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
        public async Task UpdateProfile([FromBody] CurrentUser updatedUser)
        {
            await _userService.UpdateProfile(updatedUser);
            
        }

        [ApiAuthentication]
        [HttpPost(Name = "UpdatePassword")]
        public async Task UpdatePassword([FromBody] ChangePasswordRequest request)
        {
            await _userService.UpdatePassword(request);

        }
    }
}
