using Data.Database;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Logic.UserService.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.UserService
{
    public class UserService : IUserService
    {
        private readonly Logger<UserService> _logger;
        private readonly IUserUnitOfWork _userUnitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogicBase _logicBase;

        public UserService(
            DatabaseContext dbContext,
            ILogicBase logicBase,
            IUserUnitOfWork userUnitOfWork,
            IJwtTokenService jwtTokenService,
            IOptions<JwtTokenModel> jwtOptions)
        {
            _logger = new Logger<UserService>(dbContext);
            _userUnitOfWork = userUnitOfWork;
            _jwtTokenService = jwtTokenService;
            _logicBase = logicBase;
        }

        public async Task<AuthenticationResult> AuthenticateUser(AuthenticationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new AuthenticationResult
                    {
                        Result = false,
                    };
                }

                var userEntity = await _userUnitOfWork.UserRepository.FirstOrDefaultAsync(user =>
                    user.EmailAddress == request.Email,
                    false, x => x.UserCredentials);

                if (userEntity == null || userEntity?.UserCredentials == null)
                {
                    return new AuthenticationResult
                    {
                        Result = false,
                    };
                }

                if (!PasswordHasher.VerifyPassword(request.Password, userEntity.UserCredentials.PasswordHash))
                {
                    return new AuthenticationResult
                    {
                        Result = false,
                    };
                }

                var accessToken = _jwtTokenService.GenerateTokens(userEntity);

                userEntity.UserCredentials.RefreshToken = accessToken.RefreshToken;
                userEntity.UserCredentials.ExpireDate = DateTime.Now.AddSeconds(_jwtTokenService.GetJwtExpireSeconds());

                await _userUnitOfWork.SaveChangesAsync("System");

                return new AuthenticationResult
                {
                    Result = true,
                    AccessToken = accessToken.Jwt,
                    RefeshToken = accessToken.RefreshToken
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("Authentication failded.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return new AuthenticationResult
                {
                    Result = false,
                };
            }
        }

        public async Task<bool> Logout(string idExternal)
        {
            try
            {
                var currentUser = _logicBase.GetCurrentUser();

                var userEntity = await _userUnitOfWork.UserRepository.FirstOrDefaultAsync(user =>
                    user.EmailAddress == currentUser.EmailAddress,
                    false, x => x.UserCredentials);

                if (userEntity == null || userEntity?.UserCredentials == null)
                {
                    return false;
                }

                userEntity.UserCredentials.RefreshToken = string.Empty;
                userEntity.UserCredentials.ExpireDate = DateTime.MinValue;

                await _userUnitOfWork.SaveChangesAsync("System");

                return true;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("Logout failded.",
                    LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return false;
            }
        }

    }
}


      

      