using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boiler.Core.Abstractions;

public interface ILocalEventBus: ISingletonDependency
{
    Task PublishMessageAsync<T>(T eventObject);
    Task PublishMessagesAsync<T>(IEnumerable<T> eventObject);
    Task Subscribe<T>(Action<T> callback);
    Task UnSubscribe<T>(Action<T> callback);
}