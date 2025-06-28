using Infant.Core.Models.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infant.Data.EntityFrameworkCore;

public abstract class EfRepositoryBase<T> where T : class, IEntity
{
    private readonly DbContext _dbContext;
    private readonly bool _softDeletable = typeof(T).IsAssignableTo(typeof(ISoftDeletable));
    private readonly bool _mayHaveTenant = typeof(T).IsAssignableTo(typeof(IMayHaveTenant));
    private bool? _includeDeleted = null;
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
        if (_softDeletable && (!includeDeleted))
        {
            return _dbContext.Set<T>().NotDeleted();
        }

        return _dbContext.Set<T>();
    }

    public EfRepositoryBase<T> SetIncludeDeleted(bool includeDeleted)
    {
        this._includeDeleted = includeDeleted;
        return this;
    }
    
    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}