using Data.Database;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.UserService.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.UserService
{
    public class UserService : ALogicBase, IUserService
    {
        private readonly Logger<UserService> _logger;
        private readonly IUserUnitOfWork _userUnitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        public UserService(
            DatabaseContext dbContext,
            IUserUnitOfWork userUnitOfWork,
            IJwtTokenService jwtTokenService,
            IOptions<JwtTokenModel> jwtOptions) : base(dbContext)
        {
            _logger = new Logger<UserService>(dbContext);
            _userUnitOfWork = userUnitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthenticationResult> AuthenticateUser(AuthenticationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password) || request.IdExternal == Guid.Empty)
                {
                    return new AuthenticationResult
                    {
                        Result = false,
                    };
                }

                var userEntity = await _userUnitOfWork.UserRepository.FirstOrDefaultAsync(user =>
                    user.UserIdExternal == request.IdExternal &&
                    user.UserName == request.UserName,
                    false, x => x.UserCredentials);

                if (userEntity == null)
                {
                    return new AuthenticationResult
                    {
                        Result = false,
                    };
                }

                var passwordHash = GetPasswordHash(request.Password, userEntity.UserCredentials.Salt);

                if (passwordHash != userEntity.UserCredentials.PasswordHash)
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
                var userEntity = await _userUnitOfWork.UserRepository.FirstOrDefaultAsync(user =>
                    user.UserIdExternal.ToString() == idExternal,
                    false, x => x.UserCredentials);
                
                if (userEntity == null)
                {
                    return false;
                }

                userEntity.UserCredentials.RefreshToken = string.Empty;
                
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
