namespace OrderManagement.Infrastructure.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>() where TEntity : class, IStatus;
    }
}
