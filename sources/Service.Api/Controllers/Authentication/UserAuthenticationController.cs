using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Authentication;

namespace Service.Api.Controllers.Authentication
{
    public class UserAuthenticationController : ApiControllerBase
    {
        private readonly IUserService _userService;
        
        public UserAuthenticationController(IUserService userService) 
        { 
            _userService = userService;
        }

        [HttpPost(Name = "AuthenticateUser")]
        public async Task<AuthenticationResult> AuthenticateUser([FromBody] AuthenticationRequest request)
        {
            return await _userService.AuthenticateUser(request);
        }

        [ApiAuthentication]
        [HttpPost(Name = "LogoutUser")]
        public async Task<bool> LogoutUser([FromQuery] string idExternal)
        {
            return await _userService.Logout(idExternal);
        }
    }
}
