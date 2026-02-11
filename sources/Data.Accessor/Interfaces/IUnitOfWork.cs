using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;

namespace Data.Accessor.Interfaces
{
    public interface IUnitOfWork
    {
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        IRepositoryBase<UserSettingsEntity> UserSettingsRepository { get; }
        IRepositoryBase<ImportFileEntity> ImportFileRepository { get; }
        IRepositoryBase<PartOfSpeechEntity> PartOfSpeechRepository { get; }
        IRepositoryBase<LanguageEntity> LanguageRepository { get; }
        IRepositoryBase<VocabularyEntity> VocabularyRepository { get; }
        IRepositoryBase<VocabularyTopicEntity> VocabularyTopicRepository { get; }
        IRepositoryBase<ScheduledTaskEntity> ScheduledTaskRepository { get; }
       
        Task<int> SaveChangesAsync(string userName);
    }
}
