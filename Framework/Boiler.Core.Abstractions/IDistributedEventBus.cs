using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boiler.Core.Abstractions;

public interface IDistributedEventBus
{
    Task PublishMessageAsync<T>(T eventObject);
    Task PublishMessagesAsync<T>(IEnumerable<T> eventObjects);
}