using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;

namespace Data.Accessor.Interfaces
{
    public interface IUnitOfWork
    {
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        IRepositoryBase<UserSettingsEntity> UserSettingsRepository { get; }
        IRepositoryBase<ImportFileEntity> ImportFileRepository { get; }
        public IVocabularyUnitOfWork VocabularyUnitOfWork { get; }
        Task<int> SaveChangesAsync(string userName);
    }
}
