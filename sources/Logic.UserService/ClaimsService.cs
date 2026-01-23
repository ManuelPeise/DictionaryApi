using Data.Database.Entities.User;
using System.Security.Claims;

namespace Logic.UserService
{
    public static class ClaimsService
    {
        public static List<Claim> GetUserClaims(UserEntity userEntity)
        {
            return new List<Claim>
            {
                new Claim("UserName", userEntity.UserName),
                new Claim("UserId", userEntity.Id.ToString()),
                new Claim("UserRole", userEntity.UserRole.ToString()),
                new Claim("ExpireTime", DateTime.UtcNow.AddHours(1).ToString("o"))
            };
        }
    }
}
