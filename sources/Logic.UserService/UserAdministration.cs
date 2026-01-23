using Data.Database;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.UserService.Interfaces;
using Shared.Enums;
using Shared.Models.Administration;

namespace Logic.UserService
{
    public class UserAdministration : ALogicBase, IUserAdministration
    {
        private readonly Logger<UserAdministration> _logger;
        private readonly IUserUnitOfWork _userUnitOfWork;

        public UserAdministration(DatabaseContext dbContect, IUserUnitOfWork userUnitOfWork) : base(dbContect)
        {
            _logger = new Logger<UserAdministration>(dbContect);
            _userUnitOfWork = userUnitOfWork;
        }

        public async Task<UserRegistrationResult?> RegisterUser(UserRegistrationRequestModel requestModel)
        {
            try
            {
                var existingUserEntity = await _userUnitOfWork.UserRepository
                    .FirstOrDefaultAsync(user => user.EmailAddress == requestModel.EmailAddress, false);

                if(existingUserEntity != null)
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
                    UserIdExternal = Guid.NewGuid(),
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    DateOfBirth = requestModel.DateOfBirth,
                    ProfileImage = Array.Empty<byte>(),
                    EmailAddress = requestModel.EmailAddress,
                    UserRole = UserRoleEnum.User,
                    UserCredentials = new UserCredentialsEntity
                    {
                        PasswordHash = GetPasswordHash(requestModel.Password, salt),
                        Salt = salt,
                        RefreshToken = null,
 
                    },
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                };

                var result = await _userUnitOfWork.UserRepository.AddAsync(newUserEntity);

                if(result > 0)
                {
                    await _userUnitOfWork.SaveChangesAsync("System");
                }

                return new UserRegistrationResult
                {
                    Result = result > 0,
                    Message = result > 0 ? "User registered successfully." : "Failed to register user."
                };

            }
            catch(Exception exception)
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
