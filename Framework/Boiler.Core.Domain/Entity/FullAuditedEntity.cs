using System;

namespace Boiler.Core.Domain.Entity;

public abstract class FullAuditedEntity<TKey> : IEntity<TKey>, ISoftDeletable, IHasDeletedOn, ICreationAudited, IModificationAudited
    where TKey : struct
{
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdaterId { get; set; }
    public bool IsDeleted { get; set; }
    public TKey Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? DeletedOn { get; set; }
}