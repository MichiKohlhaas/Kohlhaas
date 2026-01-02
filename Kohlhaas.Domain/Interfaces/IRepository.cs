using System.Linq.Expressions;
using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task<TEntity> Insert(TEntity entity);
    IQueryable<TEntity> AsQueryable();
    Task<IEnumerable<TEntity>> Insert(IEnumerable<TEntity> entities);
    /// <summary>
    /// Returns nothing if entity is considered deleted.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TEntity?> GetById(Guid id);
    /// <summary>
    /// Excludes soft deleted entities.
    /// </summary>
    /// <returns></returns>
    Task<ICollection<TEntity>> GetAll();
    Task<(ICollection<TEntity> Items, int TotalCount, int TotalPages)> GetPagedData(int pageNumber, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
    /// <summary>
    /// Lambda expressions to allow calling code to specify query.
    /// </summary>
    /// <param name="predicate">filter condition</param>
    /// <param name="orderBy">column to order by</param>
    /// <returns>A collection of the specified entity type</returns>
    Task<ICollection<TEntity>> Get(Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    Task<TEntity?> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    
    Task<int> Count();
    Task<int> Count(Expression<Func<TEntity, bool>> predicate);
    Task<bool> Any(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>True if updated</returns>
    Task Update(TEntity entity);
    /// <summary>
    /// Bulk updates entities.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns>Count updated</returns>
    Task Update(IEnumerable<TEntity> entities);
    Task HardDelete(TEntity entity);
    Task SoftDelete(TEntity entity);
    Task HardDelete(Guid id);
    Task SoftDelete(Guid id);
    /// <summary>
    /// Bulk hard deletes entities
    /// </summary>
    /// <param name="entities"></param>
    Task HardDelete(IEnumerable<TEntity> entities);
    /// <summary>
    /// Bulk soft deletes entities
    /// </summary>
    /// <param name="entities"></param>
    Task SoftDelete(IEnumerable<TEntity> entities);
}