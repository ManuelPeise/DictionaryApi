using Data.Database.Entities.User;
using Shared.Models.Authentication;

namespace Logic.UserService.Interfaces
{
    public interface IJwtTokenService
    {
        (string Jwt, string RefreshToken) GenerateTokens(UserEntity user);
        Task<TokenModel> RefreshToken(TokenModel request);
        int GetJwtExpireSeconds();
        JwtTokenModel GetJwtOptions();
    }
}
