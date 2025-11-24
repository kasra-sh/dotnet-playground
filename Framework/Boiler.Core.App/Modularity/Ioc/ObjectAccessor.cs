#nullable enable
using Boiler.Core.Abstractions;

namespace Boiler.Core.Modularity.Ioc;

public class ObjectAccessor<T> : IObjectAccessor<T>
{
    public T? Value { get; set; }

    public ObjectAccessor()
    {

    }

    public ObjectAccessor(T obj)
    {
        Value = obj;
    }
}