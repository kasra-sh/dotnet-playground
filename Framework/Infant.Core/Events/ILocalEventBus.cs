using Infant.Core.DI;
using Infant.Core.Models.Ddd;

namespace Infant.Core.Events;

public interface ILocalEventBus: ISingletonDependency
{
    Task PublishMessageAsync<T>(T eventObject);
    Task PublishMessagesAsync<T>(IEnumerable<T> eventObject);
    Task Subscribe<T>(Action<T> callback);
    Task UnSubscribe<T>(Action<T> callback);
}