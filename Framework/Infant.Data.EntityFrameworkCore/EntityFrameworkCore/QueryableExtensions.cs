using System.Linq.Dynamic.Core;
using Infant.Core.Models.Domain.Entity;
using Infant.Core.Models.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infant.Data.EntityFrameworkCore;

public static class QueryableExtensions
{
    public static IQueryable<T> Paged<T>(this IQueryable<T> queryable, int maxCount, int pageNum) where T : class, IEntity
    {
        return queryable.Skip(pageNum * maxCount).Take(maxCount);
    }

    /// <summary>
    /// An extension to standardize getting paged result from EntityFramework's IQueryable
    /// </summary>
    /// <param name="queryable">EntityFramework's IQueryable to get paged results from</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageNum">Page number starts from 0</param>
    /// <typeparam name="T">A class implementing IEntity</typeparam>
    /// <returns></returns>
    public static async Task<PagedResultDto<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, int pageSize, int pageNum, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        return new PagedResultDto<T>
        {
            TotalCount = await queryable.CountAsync(cancellationToken),
            Items = await queryable.Paged<T>(pageSize, pageNum).ToListAsync(cancellationToken),
            PageNum = pageNum,
            PageSize = pageSize
        };
    }

    public static IQueryable<T> WhereDynamic<T>(this IQueryable<T> queryable, string predicate, params object?[] args) where T: IEntity
    {
        // if (args.Length > 0)
        return queryable.Where(predicate, args);
        // return queryable.Where(predicate);
    }

    public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> queryable, string ordering) where T: IEntity
    {
        return queryable.OrderBy(ordering);
    }

    public static IQueryable<T> NotDeleted<T>(this IQueryable<T> queryable) where T : IEntity, ISoftDeletable
    {
        return queryable.Where(e => e.IsDeleted != true);
    }
}