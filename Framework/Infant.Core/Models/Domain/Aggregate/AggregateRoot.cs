using Infant.Core.Models.Domain.Entity;

namespace Infant.Core.Models.Domain.Aggregate;

public abstract class AggregateRoot : OutBoxObject, IEntity
{
    
}

public abstract class AggregateRoot<TKey> : OutBoxObject, IEntity<TKey>
{
    public TKey Id { get; set; } = default;
}