using System.Linq.Expressions;

namespace OrderManagement.Infrastructure.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IStatus
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<bool> AreAllIdsFoundAsync(IEnumerable<int> ids);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity?> GetByIdWithIncludesAsync(int id, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity?>> GetAllWithIncludesAsync(params Expression<Func<TEntity, object>>[] includes);
    }
}
