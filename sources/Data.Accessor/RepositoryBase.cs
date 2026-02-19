using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Accessor
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : AEntityBase
    {
        private DatabaseContext _context;
        private readonly DbSet<TEntity> _table;

        public RepositoryBase(DatabaseContext context)
        {
            _context = context;
            _table = _context.Set<TEntity>();
        }

        public Task<HashSet<TEntity>> GetAllAsync(
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null)
        {
            var table = _table.AsQueryable();

            if (asNoTracking)
            {
                table = table.AsNoTracking();
            }

            if (includeExpression != null)
            {
                table = table.Include(includeExpression);
            }

            return table.ToHashSetAsync();
        }

        public Task<HashSet<TEntity>> GetAllByAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            Expression<Func<TEntity, object>>? includeExpression = null,
            bool asNoTracking = false)
        {
            var table = _table.AsQueryable();

            if (asNoTracking)
            {
                table = table.AsNoTracking();
            }

            if (includeExpression != null)
            {
                table = table.Include(includeExpression);
            }

            return table.Where(whereExpression).ToHashSetAsync();
        }

        public Task<TEntity?> FirstOrDefaultByIdAsync(
            int id,
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null)
        {
            var table = _table.AsQueryable();

            if (asNoTracking)
            {
                table = table.AsNoTracking();
            }

            if (includeExpression != null)
            {
                table = table.Include(includeExpression);
            }

            return table.FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<TEntity?> FirstOrDefaultByIdExternalAsync(
            Guid idExternal, 
            bool asNoTracking = false, 
            Expression<Func<TEntity, object>>? includeExpression = null)
        {
            var table = _table.AsQueryable();
            if (asNoTracking)
            {
                table = table.AsNoTracking();
            }
            if (includeExpression != null)
            {
                table = table.Include(includeExpression);
            }

            return table.FirstOrDefaultAsync(e => e.IdExternal == idExternal);
        }

        public Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null)
        {
            var table = _table.AsQueryable();

            if (asNoTracking)
            {
                table = table.AsNoTracking();
            }

            if (includeExpression != null)
            {
                table = table.Include(includeExpression);
            }

            return table.FirstOrDefaultAsync(whereExpression);
        }

        public async Task<int> AddAsync(
            TEntity entity,
            Expression<Func<TEntity, bool>>? whereExpression = null)
        {
            var table = _table.AsQueryable();

            var isExisting = whereExpression == null ? false : table.Any(whereExpression);

            if (isExisting)
            {
                return await Task.FromResult(0);
            }

            var result = await _table.AddAsync(entity);

            return await Task.FromResult(result.Entity.Id);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _table.AddRangeAsync(entities);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var table = _table.AsQueryable();

            await Task.Run(async () => await table.ExecuteUpdateAsync(e => e.SetProperty(p => p, entity)));
        }

        public async Task BulkUpdateAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _table.UpdateRange(entities));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => _table.Remove(entity));
        }

        public async Task BulkDelete(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _table.RemoveRange(entities));
        }
    }
}
