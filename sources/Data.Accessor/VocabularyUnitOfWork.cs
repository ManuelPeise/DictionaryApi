using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;


namespace Data.Accessor
{
    public class VocabularyUnitOfWork: IVocabularyUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        private readonly IRepositoryBase<LanguageEntity> _languageRepository;
        public IRepositoryBase<LanguageEntity> LanguageRepository => _languageRepository ?? new RepositoryBase<LanguageEntity>(_dbContext);

        private readonly IRepositoryBase<PartOfSpeechEntity> _partOfSpeechRepository;
        public IRepositoryBase<PartOfSpeechEntity> PartOfSpeechRepository => _partOfSpeechRepository ?? new RepositoryBase<PartOfSpeechEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyEntity> _vocabularyRepository;
        public IRepositoryBase<VocabularyEntity> VocabularyRepository => _vocabularyRepository ?? new RepositoryBase<VocabularyEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyCategoryEntity> _vocabularyCategoryRepository;
        public IRepositoryBase<VocabularyCategoryEntity> VocabularyCategoryRepository => _vocabularyCategoryRepository ?? new RepositoryBase<VocabularyCategoryEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyToCategoryEntity> _vocabularyToCategoryRepository;
        public IRepositoryBase<VocabularyToCategoryEntity> VocabularyToCategoryRepository => _vocabularyToCategoryRepository ?? new RepositoryBase<VocabularyToCategoryEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularySessionEntity> _vocabularySessionRepository;
        public IRepositoryBase<VocabularySessionEntity> VocabularySessionRepository => _vocabularySessionRepository ?? new RepositoryBase<VocabularySessionEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularySessionResultEntity> _vocabularySessionResultRepository;
        public IRepositoryBase<VocabularySessionResultEntity> VocabularySessionResultRepository => _vocabularySessionResultRepository ?? new RepositoryBase<VocabularySessionResultEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyProgressEntity> _vocabularyProgressRepository;
        public IRepositoryBase<VocabularyProgressEntity> VocabularyProgressRepository => _vocabularyProgressRepository ?? new RepositoryBase<VocabularyProgressEntity>(_dbContext);

        public VocabularyUnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _languageRepository = new RepositoryBase<LanguageEntity>(_dbContext);
            _partOfSpeechRepository = new RepositoryBase<PartOfSpeechEntity>(_dbContext);
            _vocabularyRepository = new RepositoryBase<VocabularyEntity>(_dbContext);
            _vocabularyToCategoryRepository = new RepositoryBase<VocabularyToCategoryEntity>(_dbContext);
            _vocabularyCategoryRepository = new RepositoryBase<VocabularyCategoryEntity>(_dbContext);
            _vocabularySessionRepository = new RepositoryBase<VocabularySessionEntity>(_dbContext);
            _vocabularySessionResultRepository = new RepositoryBase<VocabularySessionResultEntity>(_dbContext);
            _vocabularyProgressRepository = new RepositoryBase<VocabularyProgressEntity>(_dbContext);
        }

        public async Task<int> SaveChangesAsync(string userName)
        {
            if (_dbContext == null) throw new ObjectDisposedException(nameof(UnitOfWork));

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
