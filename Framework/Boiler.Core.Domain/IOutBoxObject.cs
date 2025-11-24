using System.Collections.Generic;

namespace Boiler.Core.Domain;

public interface IOutBoxObject
{
    ICollection<object> GetLocalEvents();
    ICollection<object> GetDistributedEvents();
    void ClearLocalEvents();
    void ClearDistributedEvents();
}