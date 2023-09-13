namespace Infant.Core.Models.Domain;

public interface IOutBoxObject
{
    ICollection<object> GetLocalEvents();
    ICollection<object> GetDistributedEvents();
    void ClearLocalEvents();
    void ClearDistributedEvents();
}