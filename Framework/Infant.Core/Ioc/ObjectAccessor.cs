#nullable enable
namespace Infant.Core.Ioc;
public interface IObjectAccessor<out T>
{
    T? Value { get; }
}
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