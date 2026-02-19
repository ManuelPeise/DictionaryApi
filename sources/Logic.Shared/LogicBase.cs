using Data.Accessor.Interfaces;
using Data.Database;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Enums;
using Shared.Models.Settings;
using Shared.Models.User;

namespace Logic.Shared
{
    public class LogicBase: ILogicBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        protected IUnitOfWork UnitOfWork => _unitOfWork;
        
        public LogicBase(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public UserModel GetCurrentUser(bool? includeDetails = false)
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

            var currentUser = new UserModel
            {
                Id = userEntity.Id,
                IdExternal = userEntity.IdExternal,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                EmailAddress = userEntity.EmailAddress,
                ProfileImage = userEntity.ProfileImage,
                DateOfBirth = userEntity.DateOfBirth,
                UserRole = userEntity.UserRole,
                UserCredentialsId = userEntity.UserCredentialsId,
                UserCredentials = new UserCredentials
                {
                    PasswordHash = userEntity.UserCredentials?.PasswordHash ?? string.Empty,
                    RefreshToken = userEntity.UserCredentials?.RefreshToken,
                    ExpireDate = userEntity.UserCredentials?.ExpireDate ?? DateTime.MinValue
                },
                UserSettingsId = userEntity.UserSettingsId,
                UserSettings = new UserSettings
                {
                    Culture = userEntity.UserSettings?.Culture ?? CultureEnum.English,
                    IsAutoDataSyncEnabled = userEntity.UserSettings?.IsAutoDataSyncEnabled ?? false,
                    UseLocalDataStore = userEntity.UserSettings?.UseLocalDataStore ?? false
                },
                CreatedAt = userEntity.CreatedAt,
                CreatedBy = userEntity.CreatedBy,
                UpdatedAt = userEntity.UpdatedAt,
                UpdatedBy = userEntity.UpdatedBy
            };

            return currentUser;
        }
    }
}
