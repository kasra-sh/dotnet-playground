using System;
using Boiler.Core.Domain.Entity;

namespace Boiler.Core.Domain.Aggregate;

public abstract class FullAuditedAggregateRoot : OutBoxObject, IEntity, IFullAudited
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdaterId { get; set; }
}

public abstract class FullAuditedAggregateRoot<TKey> : OutBoxObject, IEntity<TKey>, IFullAudited
{
    public TKey Id { get; set; } = default;
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdaterId { get; set; }
}