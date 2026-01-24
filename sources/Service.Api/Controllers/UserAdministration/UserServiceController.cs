using Logic.Shared.Models;
using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<CurrentUser?> GetCurrentUser()
        {
            var result = await _userService.GetCurrentUserData();

            return result;
        }
    }
}
