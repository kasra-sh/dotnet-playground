namespace Infant.Core.Models.Domain.Entity;

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

public interface IFullAudited : ISoftDeletable, ICreationAudited, IModificationAudited
{
}

public abstract class FullAuditedObject : IFullAudited
{
    public DateTime? UpdatedOn { get; set; }
    public Guid? UpdaterId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatorId { get; set; }
}

public interface ICreationAuditedEntity<TKey> : IEntity<TKey>, ISoftDeletable, ICreationAudited
    where TKey : struct
{
}

public interface ICreationAuditedEntity : IEntity, ISoftDeletable, ICreationAudited
{
}

public interface IModificationAudited : IHasUpdatedOn, IMayHaveUpdater
{
}

public interface ICreationAudited : IHasCreatedOn, IMayHaveCreator
{
}

public interface IEntity
{
    
}
public interface IEntity<TKey>: IEntity
{
    public TKey Id { get; set; }
}

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}

public interface IHasDeletedOn
{
    public DateTime? DeletedOn { get; set; }
}

public interface IMayHaveUpdater
{
    public Guid? UpdaterId { get; set; }
}

public interface IHasUpdatedOn
{
    public DateTime? UpdatedOn { get; set; }
}

public interface IMayHaveCreator
{
    public Guid? CreatorId { get; set; }
}

public interface IHasCreatedOn
{
    public DateTime CreatedOn { get; set; }
}