namespace Boiler.Core.Domain.Entity;

public interface IEntity<TKey>: IEntity
{
    public TKey Id { get; set; }
}

public interface IEntity
{
}