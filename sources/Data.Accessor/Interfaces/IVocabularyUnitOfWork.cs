using Data.Database.Entities.Vocabulary;


namespace Data.Accessor.Interfaces
{
    public interface IVocabularyUnitOfWork
    {
        IRepositoryBase<PartOfSpeechEntity> PartOfSpeechRepository { get; }
        IRepositoryBase<LanguageEntity> LanguageRepository { get; }
        IRepositoryBase<VocabularyEntity> VocabularyRepository { get; }
        IRepositoryBase<VocabularyCategoryEntity> VocabularyCategoryRepository { get; }
        IRepositoryBase<VocabularyToCategoryEntity> VocabularyToCategoryRepository { get; }
        IRepositoryBase<VocabularySessionEntity> VocabularySessionRepository { get; }
        IRepositoryBase<VocabularySessionResultEntity> VocabularySessionResultRepository { get; }
        IRepositoryBase<VocabularyProgressEntity> VocabularyProgressRepository { get; }

        Task<int> SaveChangesAsync(string userName);
    }
}
