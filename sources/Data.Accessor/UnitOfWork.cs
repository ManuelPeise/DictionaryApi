using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace Data.Accessor
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        // user unit of work
        private IRepositoryBase<UserEntity>? _userRepository;
        public IRepositoryBase<UserEntity> UserRepository => _userRepository ?? new RepositoryBase<UserEntity>(_dbContext);

        private IRepositoryBase<UserCredentialsEntity>? _userCredentialsRepository;
        public IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository => _userCredentialsRepository ?? new RepositoryBase<UserCredentialsEntity>(_dbContext);

        private IRepositoryBase<UserSettingsEntity>? _userSettingsRepository;
        public IRepositoryBase<UserSettingsEntity> UserSettingsRepository => _userSettingsRepository ?? new RepositoryBase<UserSettingsEntity>(_dbContext);

        // vocabulary

        private readonly IRepositoryBase<LanguageEntity> _languageRepository;
        public IRepositoryBase<LanguageEntity> LanguageRepository => _languageRepository ?? new RepositoryBase<LanguageEntity>(_dbContext);

        private readonly IRepositoryBase<PartOfSpeechEntity> _partOfSpeechRepository;
        public IRepositoryBase<PartOfSpeechEntity> PartOfSpeechRepository => _partOfSpeechRepository ?? new RepositoryBase<PartOfSpeechEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyEntity> _vocabularyRepository;
        public IRepositoryBase<VocabularyEntity> VocabularyRepository => _vocabularyRepository ?? new RepositoryBase<VocabularyEntity>(_dbContext);

        private readonly IRepositoryBase<VocabularyTopicEntity> _vocabularyTopicRepository;
        public IRepositoryBase<VocabularyTopicEntity> VocabularyTopicRepository => _vocabularyTopicRepository ?? new RepositoryBase<VocabularyTopicEntity>(_dbContext);
        // files unit of work

        private IRepositoryBase<ImportFileEntity>? _importFileRepository;
        public IRepositoryBase<ImportFileEntity> ImportFileRepository => _importFileRepository ?? new RepositoryBase<ImportFileEntity>(_dbContext);

        private IRepositoryBase<ScheduledTaskEntity>? _scheduledTaskRepository;
        public IRepositoryBase<ScheduledTaskEntity> ScheduledTaskRepository => _scheduledTaskRepository ?? new RepositoryBase<ScheduledTaskEntity>(_dbContext);

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _userRepository = new RepositoryBase<UserEntity>(_dbContext);
            _userCredentialsRepository = new RepositoryBase<UserCredentialsEntity>(_dbContext);
            _userSettingsRepository = new RepositoryBase<UserSettingsEntity>(_dbContext);
            _importFileRepository = new RepositoryBase<ImportFileEntity>(_dbContext);
            _scheduledTaskRepository = new RepositoryBase<ScheduledTaskEntity>(_dbContext);
            _languageRepository = new RepositoryBase<LanguageEntity>(_dbContext);
            _partOfSpeechRepository = new RepositoryBase<PartOfSpeechEntity>(_dbContext);
            _vocabularyRepository = new RepositoryBase<VocabularyEntity>(_dbContext);
            _vocabularyTopicRepository = new RepositoryBase<VocabularyTopicEntity>(_dbContext);

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
