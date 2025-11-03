using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace OrderManagement.Infrastructure
{
    public class Repository<TContext, TEntity>(TContext context) : IRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, IStatus
    {
        private readonly TContext _context = context;
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbSet.AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<bool> AreAllIdsFoundAsync(IEnumerable<int> ids)
        {
            // Si può ottimizzare gestendo in batch con una richiesta unica per tutti gli id
            foreach (var id in ids)
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return false;
                if (entity.Status == Extensions.CurrentStatus.Refused
                    || entity.Status == Extensions.CurrentStatus.Draft)
                    return false;
            }
            return true;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity?>> GetAllWithIncludesAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeExpression in includes)
                {
                    query = query.Include(includeExpression);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdWithIncludesAsync(int id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var includeExpression in includes)
                {
                    query = query.Include(includeExpression);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

    }
}
