#nullable enable
namespace Boiler.Core.Abstractions;

public interface IObjectAccessor<out T>
{
    T? Value { get; }
}