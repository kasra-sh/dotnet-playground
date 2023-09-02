namespace Infant.Core.Events;

public interface IDistributedEventBus
{
    Task PublishMessageAsync<T>(T eventObject);
    Task PublishMessagesAsync<T>(IEnumerable<T> eventObjects);
}