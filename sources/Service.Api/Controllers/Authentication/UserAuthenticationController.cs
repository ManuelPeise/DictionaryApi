using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Models.Authentication;

namespace Service.Api.Controllers.Authentication
{
    public class UserAuthenticationController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        public UserAuthenticationController(IUserService userService, IJwtTokenService jwtTokenService) 
        { 
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost(Name = "AuthenticateUser")]
        public async Task<AuthenticationResult> AuthenticateUser([FromBody] AuthenticationRequest request)
        {
            return await _userService.AuthenticateUser(request);
        }

        [HttpPost(Name = "RefreshToken")]
        [EnableRateLimiting("fixedRateLimit")]
        public async Task<AuthenticationResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (string.IsNullOrEmpty(tokenModel.AccessToken) || string.IsNullOrEmpty(tokenModel.RefreshToken))
            {
                return new AuthenticationResult
                {
                    Result = false,
                };
            }

            var newTokens = await _jwtTokenService.RefreshToken(tokenModel);

            if (string.IsNullOrEmpty(newTokens.AccessToken) || string.IsNullOrEmpty(newTokens.RefreshToken))
            {
                return new AuthenticationResult
                {
                    Result = false,
                };
            }

            return new AuthenticationResult
            {
                Result = true,
                AccessToken = newTokens.AccessToken,
                RefeshToken = newTokens.RefreshToken
            };
        }

        [ApiAuthentication]
        [HttpPost(Name = "LogoutUser")]
        public async Task<bool> LogoutUser([FromQuery] string idExternal)
        {
            return await _userService.Logout(idExternal);
        }
    }
}
