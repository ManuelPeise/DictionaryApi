using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Data.Database.Entities.User;
using Logic.UserService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.UserService
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        private IRepositoryBase<UserEntity>? _userRepository;
        public IRepositoryBase<UserEntity> UserRepository => _userRepository ?? new RepositoryBase<UserEntity>(_dbContext);

        private IRepositoryBase<UserCredentialsEntity>? _userCredentialsRepository;
        

        public IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository => _userCredentialsRepository ?? new RepositoryBase<UserCredentialsEntity>(_dbContext);

        public UserUnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(string userName)
        {
            if (_dbContext == null) throw new ObjectDisposedException(nameof(UserUnitOfWork));

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
