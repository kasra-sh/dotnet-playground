using Infant.Core.Models.Domain;
using Infant.Core.Models.Domain.Entity;
using Infant.Core.Models.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Infant.Data.EntityFrameworkCore;

public abstract class EfCrudRepository<T, TKey> : EfRepositoryBase<T>, IEfCrudRepository<T, TKey> where T : class, IEntity<TKey>
{
    protected EfCrudRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<T?> GetById(TKey id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(e => Equals(e.Id, id), cancellationToken: cancellationToken);
    }

    public async Task<List<T>> GetListById(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(e => keys.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public async Task<PagedResultDto<T>> GetPagedList(int pageSize, int pageNum, string sorting = null, CancellationToken cancellationToken = default)
    {
        return await GetQueryable(true).WhereDynamic("IsDeleted == true").OrderBy(sorting ?? "Id ASC").ToPagedListAsync(pageSize, pageNum, cancellationToken);
    }

    public async Task<T> Insert(T entity, bool saveChangesNow = false, CancellationToken cancellationToken = default)
    {
        var addedEntity = (await GetDbSet().AddAsync(entity, cancellationToken)).Entity;
        if (saveChangesNow)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return addedEntity;
    }

    public async Task<T> UpdateOrInsert(T entity, bool saveChangesNow = false, CancellationToken cancellationToken = default)
    {
        var updatedEntity = GetDbSet().Update(entity).Entity;

        if (saveChangesNow)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return updatedEntity;
    }
}