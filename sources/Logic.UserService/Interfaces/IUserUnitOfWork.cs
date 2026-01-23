using Data.Accessor.Interfaces;
using Data.Database.Entities.User;

namespace Logic.UserService.Interfaces
{
    public interface IUserUnitOfWork
    {
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        Task<int> SaveChangesAsync(string userName);
    }
}
