using Data.Accessor.Interfaces;
using Data.Database.Entities.Vocabulary;
using Shared.Enums;

namespace Logic.Words.Interfaces
{
    public interface IWordUnitOfWork
    {
        IRepositoryBase<PartOfSpeechEntity> PartOfSpeachRepository { get; }
        IRepositoryBase<LanguageEntity> LanguageRepository { get; }
        IRepositoryBase<VocabularyEntity> VocabularyRepository { get; }
        IRepositoryBase<VocabularyTranslationEntity> TranslationRepository { get; }

        Task<int?> GetPartOfSpeachId(string? partOfSpeach);
        Task<int?> GetLanguageId(LanguageEnum language);
        Task<int> SaveChangesAsync(string userName);
    }
}
