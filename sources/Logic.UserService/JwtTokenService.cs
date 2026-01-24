using Data.Database;
using Data.Database.Entities.User;
using Logic.UserService.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Logic.UserService
{
    internal class JwtTokenService : IJwtTokenService
    {
        private readonly IOptions<JwtTokenModel> _jwtOptions;
        private readonly IUserUnitOfWork _unitOfWork;

        public JwtTokenService(
            DatabaseContext dbContext,
            IUserUnitOfWork unitOfWork, 
            IOptions<JwtTokenModel> jwtOptions)
        {
            _jwtOptions = jwtOptions;
            _unitOfWork = unitOfWork;
        }

        public (string Jwt, string RefreshToken) GenerateTokens(UserEntity user)
        {
            return (GenerateJwt(user), GenerateRefreshToken());
        }

        public async Task<TokenModel> RefreshToken(TokenModel request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity!.Name;

            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(x => x.UserName == username, false, x => x.UserCredentials);

            if (user == null || user?.UserCredentials == null || user.UserCredentials.RefreshToken != request.RefreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newAccessToken = GenerateJwt(user);
            var newRefreshToken = GenerateRefreshToken();

            user.UserCredentials.RefreshToken = newRefreshToken;

            await _unitOfWork.UserCredentialsRepository.UpdateAsync(user.UserCredentials);

            await _unitOfWork.SaveChangesAsync("System");

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        public int GetJwtExpireSeconds()
        {
            return _jwtOptions.Value.ExpiresInSeconds;
        }

        public JwtTokenModel GetJwtOptions()
        {
            return _jwtOptions.Value;
        }

        private string GenerateJwt(UserEntity appUserEntity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecurityKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = ClaimsService.GetUserClaims(appUserEntity);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Value.ExpiresInSeconds),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Value.Audience,

                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Value.Issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtOptions.Value.SecurityKey)
                ),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
