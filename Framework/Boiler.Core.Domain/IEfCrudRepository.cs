using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boiler.Core.Domain.Entity;
using Boiler.Core.Domain.Shared;

namespace Boiler.Core.Domain;

public interface IEfCrudRepository<T, TKey> where T : class, IEntity<TKey>
{
    Task<T> GetById(TKey id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<List<T>> GetListById(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);
    Task<PagedResultDto<T>> GetPagedList(int pageSize, int pageNum, string? sorting, CancellationToken cancellationToken = default);
}