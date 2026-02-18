using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.Files;
using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Accessor
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        // vocabulary
        private IAdministrationUnitOfWork _administrationUnitOfWork;
        public IAdministrationUnitOfWork AdministrationUnitOfWork => _administrationUnitOfWork ?? new AdministrationUnitOfWork(_dbContext);
        // user unit of work
        private IRepositoryBase<UserEntity>? _userRepository;
        public IRepositoryBase<UserEntity> UserRepository => _userRepository ?? new RepositoryBase<UserEntity>(_dbContext);

        private IRepositoryBase<UserCredentialsEntity>? _userCredentialsRepository;
        public IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository => _userCredentialsRepository ?? new RepositoryBase<UserCredentialsEntity>(_dbContext);

        private IRepositoryBase<UserSettingsEntity>? _userSettingsRepository;
        public IRepositoryBase<UserSettingsEntity> UserSettingsRepository => _userSettingsRepository ?? new RepositoryBase<UserSettingsEntity>(_dbContext);

        // vocabulary
        private IVocabularyUnitOfWork _vocabularyUnitOfWork;
        public IVocabularyUnitOfWork VocabularyUnitOfWork => _vocabularyUnitOfWork ?? new VocabularyUnitOfWork(_dbContext);

        // files unit of work

        private IRepositoryBase<ImportFileEntity>? _importFileRepository;
        public IRepositoryBase<ImportFileEntity> ImportFileRepository => _importFileRepository ?? new RepositoryBase<ImportFileEntity>(_dbContext);

       
        public UnitOfWork(DatabaseContext dbContext, IVocabularyUnitOfWork vocabularyUnitOfWork, IAdministrationUnitOfWork administrationUnitOfWork)
        {
            _dbContext = dbContext;
            _vocabularyUnitOfWork = vocabularyUnitOfWork;
            _administrationUnitOfWork = administrationUnitOfWork;
            _userRepository = new RepositoryBase<UserEntity>(_dbContext);
            _userCredentialsRepository = new RepositoryBase<UserCredentialsEntity>(_dbContext);
            _userSettingsRepository = new RepositoryBase<UserSettingsEntity>(_dbContext);
            _importFileRepository = new RepositoryBase<ImportFileEntity>(_dbContext);
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
