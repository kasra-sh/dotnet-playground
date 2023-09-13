using Infant.Core.Models.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infant.Data.EntityFrameworkCore;

public abstract class EfRepositoryBase<T> where T : class, IEntity
{
    private readonly DbContext _dbContext;
    private static readonly bool SoftDeletable = typeof(T).IsAssignableTo(typeof(ISoftDeletable));
    public EfRepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected DbContext DbContext => _dbContext;

    protected DbSet<T> GetDbSet()
    {
        return _dbContext.Set<T>();
    }

    protected IQueryable<T> GetQueryable(bool includeDeleted = false)
    {
        if ((!includeDeleted) && SoftDeletable)
        {
            return _dbContext.Set<T>().Where(entity => ((ISoftDeletable) entity).IsDeleted != true);
        }

        return _dbContext.Set<T>();
    }

    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}