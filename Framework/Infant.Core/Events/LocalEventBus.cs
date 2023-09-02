using System.Collections.Concurrent;
using Infant.Core.Models.Ddd;

namespace Infant.Core.Events;

internal class EventCallbackInfo
{
    public Action<object> Callback { get; set; }
    public Type EventType { get; set; }
}

public class LocalEventBus: ILocalEventBus
{
    private ConcurrentQueue<object> _eventObjects = new ConcurrentQueue<object>();
    private Dictionary<Type, List<Action<object>>?> _listeners = new();
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
                    foreach (var action in _listeners[result.GetType()])
                    {
                        Task.Factory.StartNew(() => action.Invoke(result));
                    }
                }
            }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness | TaskCreationOptions.DenyChildAttach);
    }
    
    public async Task PublishMessageAsync<T>(T eventObject)
    {
        _eventObjects.Enqueue(eventObject);
    }
    public async Task PublishMessagesAsync<T>(IEnumerable<T> eventObject)
    {
        
        _eventObjects.Enqueue(eventObject);
    }

    public async Task Subscribe<T>(Action<T> callback)
    {
        await _semaphoreSlim.WaitAsync();

        var callbacks = GetCallbacksForType<T>();
        
        if (!callbacks.Contains(callback))
        {
            callbacks.Add(callback);
        }
        
        _semaphoreSlim.Release();
    }

    public async Task UnSubscribe<T>(Action<T> callback)
    {
        await _semaphoreSlim.WaitAsync();
        
        var callbacks = GetCallbacksForType<T>();
        
        callbacks.Remove(callback);
        
        _semaphoreSlim.Release();
    }

    private List<Action<T>> GetCallbacksForType<T>()
    {
        var callbacks = _listeners[typeof(T)];
        
        if (callbacks == null)
        {
            callbacks = new List<Action<object>>();
            _listeners[typeof(T)] = callbacks;
        }

        return callbacks as List<Action<T>>;
    }

}