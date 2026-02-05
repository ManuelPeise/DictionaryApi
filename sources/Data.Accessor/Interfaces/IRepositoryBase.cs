using Data.Database.Entities;
using System.Linq.Expressions;

namespace Data.Accessor.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : AEntityBase
    {
        /// <summary>
        /// Asynchronously retrieves all entities of type TEntity from the data source.
        /// </summary>
        /// <param name="asNoTracking">true to disable change tracking for the returned entities; otherwise, false. Disabling tracking can improve
        /// performance when entities are read-only.</param>
        /// <param name="includeExpression">An optional expression specifying related entities to include in the query results. If null, no related
        /// entities are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a HashSet of TEntity objects
        /// representing all entities in the data source.</returns>
        Task<HashSet<TEntity>> GetAllAsync(
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null);
        /// <summary>
        /// Asynchronously retrieves all entities that satisfy the specified filter expression.
        /// </summary>
        /// <remarks>The returned entities are of type TEntity and may include related data if specified
        /// by includeExpression. When asNoTracking is true, the entities are not tracked by the underlying data
        /// context, which is recommended for scenarios where updates are not required.</remarks>
        /// <param name="whereExpression">An expression used to filter entities. Only entities for which this expression evaluates to true are
        /// returned.</param>
        /// <param name="includeExpression">An optional expression specifying related entities to include in the query results. If null, no related
        /// entities are included.</param>
        /// <param name="asNoTracking">true to retrieve entities without tracking changes in the context; otherwise, false. Use true for read-only
        /// operations to improve performance.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a set of entities matching the
        /// filter criteria. If no entities match, the set will be empty.</returns>
        Task<HashSet<TEntity>> GetAllByAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            Expression<Func<TEntity, object>>? includeExpression = null,
            bool asNoTracking = false);
        /// <summary>
        /// Asynchronously retrieves the first entity with the specified identifier, or returns null if no matching
        /// entity is found.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="asNoTracking">Specifies whether the entity should be retrieved without tracking changes in the context. Set to <see
        /// langword="true"/> to improve performance when updates are not required.</param>
        /// <param name="includeExpression">An optional expression specifying related entities to include in the query result. If null, no related
        /// entities are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity with the specified
        /// identifier, or null if no such entity exists.</returns>
        Task<TEntity?> FirstOrDefaultByIdAsync(
            int id,
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null);
        /// <summary>
        /// Asynchronously returns the first entity that matches the specified criteria, or a default value if no such
        /// entity is found.
        /// </summary>
        /// <remarks>If multiple entities match the criteria, only the first is returned. When <paramref
        /// name="asNoTracking"/> is <see langword="true"/>, the returned entity is not tracked by the context, which
        /// can improve query performance for read-only scenarios.</remarks>
        /// <param name="whereExpression">An expression used to filter entities. Only entities that satisfy this predicate are considered.</param>
        /// <param name="asNoTracking">Specifies whether the returned entity should be tracked by the context. Set to <see langword="true"/> to
        /// disable tracking for improved performance when updates are not required.</param>
        /// <param name="includeExpression">An optional expression specifying related entities to include in the query results. If <see
        /// langword="null"/>, no related entities are included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first entity that matches
        /// the criteria, or <see langword="null"/> if no entity is found.</returns>
        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            bool asNoTracking = false,
            Expression<Func<TEntity, object>>? includeExpression = null);
        /// <summary>
        /// Asynchronously adds the specified entity to the data store if no existing entity matches the given
        /// condition.
        /// </summary>
        /// <param name="entity">The entity to add to the data store. Cannot be null.</param>
        /// <param name="whereExpression">An expression used to determine whether an existing entity matches the specified condition. If a match is
        /// found, the entity will not be added.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of entities added: 1
        /// if the entity was added; otherwise, 0.</returns>
        Task<int> AddAsync(
            TEntity entity,
            Expression<Func<TEntity, bool>>? whereExpression = null);
        /// <summary>
        /// Asynchronously adds a collection of entities to the underlying data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add. Cannot be null. Each entity will be added to the data store.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of entities
        /// successfully added.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        /// <summary>
        /// Asynchronously updates the specified entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to update. Cannot be null. The entity must already exist in the data store.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>

        Task UpdateAsync(TEntity entity);
        /// <summary>
        /// Asynchronously updates a collection of entities in bulk.
        /// </summary>
        /// <param name="entities">The collection of entities to update. Cannot be null. Each entity must represent a valid, existing record to
        /// be updated.</param>
        /// <returns>A task that represents the asynchronous bulk update operation.</returns>
        Task BulkUpdateAsync(IEnumerable<TEntity> entities);
        /// <summary>
        /// Asynchronously deletes the specified entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to be deleted. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(TEntity entity);
        /// <summary>
        /// Deletes a collection of entities from the data store in a single bulk operation.
        /// </summary>
        /// <param name="entities">The collection of entities to be deleted. Cannot be null. All entities must be valid and eligible for
        /// deletion.</param>
        /// <returns>A task that represents the asynchronous bulk delete operation.</returns>
        Task BulkDelete(IEnumerable<TEntity> entities);
    }
}
