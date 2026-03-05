using Data.Database.Entities.User;

namespace Data.Accessor.Interfaces
{
    public interface IUnitOfWork
    {
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        IRepositoryBase<UserSettingsEntity> UserSettingsRepository { get; }
        IVocabularyUnitOfWork VocabularyUnitOfWork { get; }
        IAdministrationUnitOfWork AdministrationUnitOfWork { get; }
        Task<int> SaveChangesAsync(string userName);
    }
}
