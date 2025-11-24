namespace Boiler.Core.Domain.Entity;

public interface ICreationAuditedEntity : IEntity, ISoftDeletable, ICreationAudited
{
}

public interface ICreationAuditedEntity<TKey> : IEntity<TKey>, ISoftDeletable, ICreationAudited
    where TKey : struct
{
}