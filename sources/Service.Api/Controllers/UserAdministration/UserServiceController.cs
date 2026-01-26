using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.User;

namespace Service.Api.Controllers.UserAdministration
{
    public class UserServiceController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UserServiceController(IUserService userService)
        {
            _userService = userService;
        }

        [ApiAuthentication]
        [HttpGet(Name = "GetCurrentUser")]
        public async Task<UserModel?> GetCurrentUser()
        {
            var result = await _userService.GetCurrentUserData();

            return result;
        }

       
    }
}
