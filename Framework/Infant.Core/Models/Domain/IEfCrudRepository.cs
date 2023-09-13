using System.Linq.Dynamic.Core;
using Infant.Core.Models.Domain.Entity;
using Infant.Core.Models.Domain.Shared;

namespace Infant.Core.Models.Domain;

public interface IEfCrudRepository<T, TKey> where T : class, IEntity<TKey>
{
    Task<T> GetById(TKey id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<List<T>> GetListById(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);
    Task<PagedResultDto<T>> GetPagedList(int pageSize, int pageNum, string? sorting, CancellationToken cancellationToken = default);
}