namespace Boiler.Core.Domain.Entity;

public interface IFullAudited : ISoftDeletable, ICreationAudited, IModificationAudited
{
}