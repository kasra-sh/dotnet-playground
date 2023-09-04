namespace Infant.Core.Models.Ddd;

public interface IOutBoxObject
{
    ICollection<object> GetLocalEvents();
    ICollection<object> GetDistributedEvents();
    void ClearLocalEvents();
    void ClearDistributedEvents();
}