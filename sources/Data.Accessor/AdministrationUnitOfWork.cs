using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Accessor
{
    internal class AdministrationUnitOfWork: IAdministrationUnitOfWork
    {
        private readonly DatabaseContext _dbContext;

        private readonly IRepositoryBase<LogMessageEntity> _logRepository;
        public IRepositoryBase<LogMessageEntity> LogRepository => _logRepository ?? new RepositoryBase<LogMessageEntity>(_dbContext);

        public AdministrationUnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logRepository = new RepositoryBase<LogMessageEntity>(dbContext);
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
