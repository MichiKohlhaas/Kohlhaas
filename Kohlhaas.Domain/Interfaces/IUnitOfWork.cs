using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    
    Task<int> Commit(CancellationToken cancellationToken = default);
    Task Rollback(CancellationToken cancellationToken = default);
}