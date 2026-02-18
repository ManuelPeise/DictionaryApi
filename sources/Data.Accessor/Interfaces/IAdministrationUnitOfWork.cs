using Data.Database.Entities;

namespace Data.Accessor.Interfaces
{
    public interface IAdministrationUnitOfWork
    {
        public IRepositoryBase<LogMessageEntity> LogRepository { get; }
        Task<int> SaveChangesAsync(string userName);
    }
}
