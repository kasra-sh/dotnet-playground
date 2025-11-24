using System;

namespace Boiler.Core.Domain.Entity;

public abstract class FullAuditedObject : IFullAudited
{
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdaterId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatorId { get; set; }
}