namespace Boiler.Core.Domain.Entity;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}