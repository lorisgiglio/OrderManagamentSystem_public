using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Infrastructure
{
    public class RepositoryFactory<TContext> : IRepositoryFactory where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public RepositoryFactory(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<TEntity> Create<TEntity>() where TEntity : class, IStatus
        {
            return new Repository<TContext, TEntity>(_dbContext);
        }
    }

}
