namespace Infant.Core.Models.Ddd;

public interface IOutboxEntity
{
    ICollection<object> GetLocalEvents();
    ICollection<object> GetDistributedEvents();
    void ClearLocalEvents();
    void ClearDistributedEvents();
}