using System.Collections.ObjectModel;
using Infant.Core.Models.Entity;

namespace Infant.Core.Models.Ddd;

public abstract class FullAuditedAggregateRoot: FullAuditedEntity, IOutboxEntity
{
    private readonly ICollection<object> _localEvents = new List<object>(8);
    private readonly ICollection<object> _distributedEvents = new List<object>(8);

    public void AddLocalEvent(object eventData)
    {
        _localEvents.Add(eventData);
    }

    public void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }

    public ICollection<object> GetLocalEvents()
    {
        return _localEvents;
    }

    public ICollection<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    public void ClearLocalEvents()
    {
        _localEvents.Clear();
    }

    public void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }
}

public abstract class AggregateRoot : IEntity, IOutboxEntity
{
    private readonly ICollection<object> _localEvents = new List<object>(8);
    private readonly ICollection<object> _distributedEvents = new List<object>(8);

    public void AddLocalEvent(object eventData)
    {
        _localEvents.Add(eventData);
    }

    public void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }

    public ICollection<object> GetLocalEvents()
    {
        return _localEvents;
    }

    public ICollection<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    public void ClearLocalEvents()
    {
        _localEvents.Clear();
    }

    public void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }
}