using Data.Database;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Logic.UserService.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Administration;
using Shared.Models.Settings;

namespace Logic.UserService
{
    public class UserAdministration : IUserAdministration
    {
        private readonly Logger<UserAdministration> _logger;
        private readonly IUserUnitOfWork _userUnitOfWork;
        private readonly ILogicBase _logicBase;
        private readonly UserSettings _userSettings;

        public UserAdministration(DatabaseContext dbContect, IUserUnitOfWork userUnitOfWork, ILogicBase logicBase, IOptions<UserSettings> userSettings)
        {
            _logger = new Logger<UserAdministration>(dbContect);
            _userUnitOfWork = userUnitOfWork;
            _logicBase = logicBase;
            _userSettings = userSettings.Value;
        }

        public async Task<UserRegistrationResult?> RegisterUser(UserRegistrationRequestModel requestModel)
        {
            try
            {
                var existingUserEntity = await _userUnitOfWork.UserRepository
                    .FirstOrDefaultAsync(user => user.EmailAddress == requestModel.EmailAddress, false);

                if (existingUserEntity != null)
                {
                    return new UserRegistrationResult
                    {
                        Result = false,
                        Message = "User with the provided email address already exists."
                    };
                }

                var salt = Guid.NewGuid().ToString();

                var newUserEntity = new UserEntity
                {
                    UserIdExternal = Guid.NewGuid().ToString(),
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    DateOfBirth = requestModel.DateOfBirth,
                    ProfileImage = Array.Empty<byte>(),
                    EmailAddress = requestModel.EmailAddress,
                    UserRole = UserRoleEnum.User,
                    UserCredentials = new UserCredentialsEntity
                    {
                        PasswordHash = PasswordHasher.HashPassword(requestModel.Password),
                        RefreshToken = null,

                    },
                    UserSettings = new UserSettingsEntity
                    {
                        IsAutoDataSyncEnabled = _userSettings.IsSyncEnabled,
                        UseLocalDataStore = _userSettings.UseLocalDataStore,
                    },
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                };

                var result = await _userUnitOfWork.UserRepository.AddAsync(newUserEntity);

                if (result > 0)
                {
                    var currentUser = _logicBase.GetCurrentUser();
                    await _userUnitOfWork.SaveChangesAsync(currentUser.UserName);
                }

                return new UserRegistrationResult
                {
                    Result = result > 0,
                    Message = result > 0 ? "User registered successfully." : "Failed to register user."
                };

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "One or more errors occurred while register new user.",
                    LogMessageTypeEnum.Error,
                    exception.Message,
                    exception.StackTrace);

                return null;
            }
        }
    }
}
