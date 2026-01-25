using Data.Database;
using Logic.Shared.Interfaces;
using Logic.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Logic.Shared
{
    public class LogicBase: ILogicBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public LogicBase(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser(bool? includeDetails = false)
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims;

            var emailAddress = claims?.FirstOrDefault(c => c.Type == "email_address")?.Value;

            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new UnauthorizedAccessException();
            }

            var userEntity = _dbContext.UserTable.FirstOrDefault(user => user.EmailAddress == emailAddress);

            if (userEntity == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (includeDetails == true)
            {
                _dbContext.UserCredentialsTable.FirstOrDefault(uc => uc.Id == userEntity.UserCredentialsId);
                _dbContext.UserSettingsTable.FirstOrDefault(us => us.Id == userEntity.UserSettingsId);
            }

            var currentUser = new CurrentUser
            {
                Id = userEntity.Id,
                UserIdExternal = userEntity.UserIdExternal,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                EmailAddress = userEntity.EmailAddress,
                ProfileImage = userEntity.ProfileImage,
                DateOfBirth = userEntity.DateOfBirth,
                UserRole = userEntity.UserRole,
                UserCredentialsId = userEntity.UserCredentialsId,
                UserCredentials = userEntity.UserCredentials,
                UserSettingsId = userEntity.UserSettingsId,
                UserSettings = userEntity.UserSettings,
                CreatedAt = userEntity.CreatedAt,
                CreatedBy = userEntity.CreatedBy,
                UpdatedAt = userEntity.UpdatedAt,
                UpdatedBy = userEntity.UpdatedBy
            };

            return currentUser;
        }
    }
}
