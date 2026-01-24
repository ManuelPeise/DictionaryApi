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
                new Claim("user_id", userEntity.Id.ToString()),
                new Claim("user_name", userEntity.UserName),
                new Claim("email_address", userEntity.EmailAddress),
                new Claim("user_role", userEntity.UserRole.ToString()),
                new Claim("expire_time", DateTime.UtcNow.AddHours(1).ToString("o"))
            };
        }
    }
}
