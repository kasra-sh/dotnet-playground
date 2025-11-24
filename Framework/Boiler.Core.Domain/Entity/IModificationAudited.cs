namespace Boiler.Core.Domain.Entity;

public interface IModificationAudited : IHasUpdatedOn, IMayHaveUpdater
{
}