using System.Collections.Concurrent;

namespace Infant.Core.Events;

internal class Subscription
{
    public object SubscriberActionRef { get; set; }
    public Action<object> WrappedCallback { get; set; }
}

public class LocalEventBus : ILocalEventBus
{
    private ConcurrentQueue<object> _eventObjects = new ConcurrentQueue<object>();
    private ConcurrentDictionary<Type, List<Subscription>> _subscriptions = new ConcurrentDictionary<Type, List<Subscription>>();
    private SemaphoreSlim _semaphoreSlim = new(1);
    private bool IsDisposed { get; set; }

    public LocalEventBus()
    {
        Task.Factory.StartNew(async () =>
        {
            var delayMs = 1;

            while (!IsDisposed)
            {
                while (_eventObjects.IsEmpty)
                {
                    delayMs = delayMs < 100 ? delayMs + 2 : delayMs;
                    await Task.Delay(delayMs);
                }

                delayMs = 1;

                var gotEvent = _eventObjects.TryDequeue(out var result);

                if (gotEvent && result != null)
                {
                    var type = result.GetType();

                    var subscriptions = _subscriptions[type];

                    foreach (var subscription in subscriptions)
                    {
                        Task.Run(() => subscription.WrappedCallback.Invoke(result));
                    }
                }
            }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness | TaskCreationOptions.DenyChildAttach);
    }

    public async Task PublishMessageAsync<T>(T eventObject)
    {
        _eventObjects.Enqueue(eventObject);
    }

    public async Task PublishMessagesAsync<T>(IEnumerable<T> eventObjects)
    {
        foreach (var eventObj in eventObjects)
        {
            _eventObjects.Enqueue(eventObj);
        }
    }
    
    public async Task Subscribe<T>(Action<T> callback)
    {
        await _semaphoreSlim.WaitAsync();

        var subs = _subscriptions.GetOrAdd(typeof(T), type => new List<Subscription>());

        subs.Add(new()
        {
            SubscriberActionRef = callback,
            WrappedCallback = async (e) =>
            {
                callback((T) e);
            }
        });
        
        _semaphoreSlim.Release();
    }

    public async Task UnSubscribe<T>(Action<T> callback)
    {
        await _semaphoreSlim.WaitAsync();

        _subscriptions.TryGetValue(typeof(T), out var callbacks);

        var subscriberCallback = callbacks?.FirstOrDefault(c => (Action<T>) c.SubscriberActionRef == callback);
        
        if (subscriberCallback != null)
        {
            callbacks?.Remove(subscriberCallback);
        }

        _semaphoreSlim.Release();
    }
}