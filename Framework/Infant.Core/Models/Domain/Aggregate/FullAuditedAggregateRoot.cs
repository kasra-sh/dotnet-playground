using Infant.Core.Models.Domain.Entity;

namespace Infant.Core.Models.Domain.Aggregate;

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