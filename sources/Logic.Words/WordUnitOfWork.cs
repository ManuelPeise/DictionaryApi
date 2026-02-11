using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Vocabulary;
using Logic.Words.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Logic.Words
{
    public class WordUnitOfWork: IWordUnitOfWork
    {
        private readonly DatabaseContext _dbContext;
        private readonly IRepositoryBase<LanguageEntity> _languageRepository;
        public IRepositoryBase<LanguageEntity> LanguageRepository => _languageRepository ?? new RepositoryBase<LanguageEntity>(_dbContext);
        
        private readonly IRepositoryBase<PartOfSpeechEntity> _partOfSpeachRepository;
        public IRepositoryBase<PartOfSpeechEntity> PartOfSpeachRepository => _partOfSpeachRepository ?? new RepositoryBase<PartOfSpeechEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyEntity> _vocabularyRepository;
        public IRepositoryBase<VocabularyEntity> VocabularyRepository => _vocabularyRepository ?? new RepositoryBase<VocabularyEntity>(_dbContext);

        private readonly IRepositoryBase<TranslationEntity> _translationRepository;
        public IRepositoryBase<TranslationEntity> TranslationRepository => _translationRepository ?? new RepositoryBase<TranslationEntity>(_dbContext);

        public WordUnitOfWork(DatabaseContext context)
        {
            _dbContext = context;
            _languageRepository = new RepositoryBase<LanguageEntity>(context);
            _partOfSpeachRepository = new RepositoryBase<PartOfSpeechEntity>(context);
            _vocabularyRepository = new RepositoryBase<VocabularyEntity>(context);
            _translationRepository = new RepositoryBase<TranslationEntity>(context);
        }

        public async Task<int?> GetPartOfSpeachId(string? partOfSpeach)
        {
            if(string.IsNullOrEmpty(partOfSpeach))
            {
                return await Task.FromResult<int?>(null);
            }

            var entity = await PartOfSpeachRepository.FirstOrDefaultAsync(x => x.Name == partOfSpeach);

            return entity?.Id;
        }

        public async Task<int?> GetLanguageId(LanguageEnum language)
        {
            var entity = await LanguageRepository.FirstOrDefaultAsync(x => x.LanguageType == language);
           
            return entity?.Id;
        }

        public async Task<int> SaveChangesAsync(string userName)
        {
            if (_dbContext == null) throw new ObjectDisposedException(nameof(WordUnitOfWork));

            var now = DateTime.UtcNow;

            var entries = _dbContext.ChangeTracker.Entries<AEntityBase>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.CreatedBy = userName ?? string.Empty;
                    entry.Entity.UpdatedBy = userName ?? string.Empty;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(AEntityBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AEntityBase.CreatedBy)).IsModified = false;

                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userName ?? "System";
                }
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
