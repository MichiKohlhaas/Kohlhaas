using System.Data;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Interfaces;
using Kohlhaas.Infrastructure.Repositories;

namespace Kohlhaas.Infrastructure;

/// <summary>
/// Unit of work pattern.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _repositories = new Dictionary<Type, object>();
    }


    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
    {
        var type = typeof(TEntity);
        
        if (_repositories.ContainsKey(type))
        {
            return (IRepository<TEntity>)_repositories[type];
        }
        
        var repository = new EfRepository<TEntity>(_dbContext);
        _repositories.Add(type, repository);
        
        return repository;
    }

    public async Task<int> Commit(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
        }
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}