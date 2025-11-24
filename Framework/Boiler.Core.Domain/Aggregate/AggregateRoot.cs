using Boiler.Core.Domain.Entity;

namespace Boiler.Core.Domain.Aggregate;

public abstract class AggregateRoot : OutBoxObject, IEntity
{
    
}

public abstract class AggregateRoot<TKey> : OutBoxObject, IEntity<TKey>
{
    public TKey Id { get; set; } = default;
}