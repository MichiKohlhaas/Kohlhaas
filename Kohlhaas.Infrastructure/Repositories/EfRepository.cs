using System.Linq.Expressions;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kohlhaas.Infrastructure.Repositories;

public class EfRepository<TEntity>(ApplicationDbContext dbContext) : IRepository<TEntity>  where TEntity : class, IEntity
{
    public async Task<TEntity> Insert(TEntity entity)
    {
        var result = await dbContext.Set<TEntity>().AddAsync(entity);
        return result.Entity;
    }

    public async Task<IEnumerable<TEntity>> Insert(IEnumerable<TEntity> entities)
    {
        await dbContext.Set<TEntity>().AddRangeAsync(entities);
        return entities;
    }

    public async Task<TEntity?> GetById(Guid id)
    {
        return await dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ICollection<TEntity>> GetAll()
    {
        return await dbContext.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    /// Offset-based pagination. Doesn't work for scenarios where data can be concurrently modified.
    /// However, that doesn't seem likely in this situation.
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="predicate"></param>
    /// <param name="orderBy"></param>
    /// <returns></returns>
    public async Task<(ICollection<TEntity> Items, int TotalCount, int TotalPages)> GetPagedData(
        int pageNumber, 
        int pageSize, 
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>();
        
        if (predicate is not null) query = query.Where(predicate);
        if (orderBy is not null) query = orderBy(query);
        var totalItems = await query.CountAsync();
        
        var pagedItems= await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();
        
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        return (pagedItems, totalItems, totalPages);
    }

    public async Task<ICollection<TEntity>> Get(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>();
        if (predicate is not null) query = query.Where(predicate);
        if (orderBy is not null) query = orderBy(query);
        return await query.ToListAsync();
    }

    public async Task<TEntity?> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return await dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public async Task<int> Count()
    {
        return await dbContext.Set<TEntity>().CountAsync();
    }

    public async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
    {
        return await dbContext.Set<TEntity>().CountAsync(predicate: predicate);
    }

    public async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
    {
        return await dbContext.Set<TEntity>().AnyAsync(predicate);
    }
    
    public Task Update(TEntity entity)
    {
        dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task Update(IEnumerable<TEntity> entities)
    {
        dbContext.Set<TEntity>().UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task HardDelete(TEntity entity)
    {
        dbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task SoftDelete(TEntity entity)
    {
        entity.IsDeleted = true;
        dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task HardDelete(Guid id)
    {
        var entity = await dbContext.Set<TEntity>().FindAsync(id);
        if (entity is null) return;
        dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task SoftDelete(Guid id)
    {
        var entity = await dbContext.Set<TEntity>().FindAsync(id);
        if (entity is null) return;
        entity.IsDeleted = true;
        dbContext.Set<TEntity>().Update(entity);
    }

    public Task HardDelete(IEnumerable<TEntity> entities)
    {
        var enumerable = entities as TEntity[] ?? entities.ToArray();
        dbContext.Set<TEntity>().RemoveRange(enumerable);
        return Task.CompletedTask;
    }

    public Task SoftDelete(IEnumerable<TEntity> entities)
    {
        var enumerable = entities as TEntity[] ?? entities.ToArray();
        foreach (var entity in enumerable)
        {
            entity.IsDeleted = true;
        }
        dbContext.Set<TEntity>().UpdateRange(enumerable);
        return Task.CompletedTask;
    }
}