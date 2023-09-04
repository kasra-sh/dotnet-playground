using System.Collections.ObjectModel;
using Infant.Core.Models.Entity;

namespace Infant.Core.Models.Ddd;

public abstract class AggregateRoot : OutBoxObject, IEntity
{
    
}

public abstract class AggregateRoot<TKey> : OutBoxObject, IEntity<TKey>
{
    public TKey Id { get; set; } = default;
}