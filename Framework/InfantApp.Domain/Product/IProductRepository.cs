using Infant.Core.DI;

namespace InfantApp.Domain;

public interface IProductRepository: ITransientDependency
{
    Task<bool> InsertProductsAsync();
    Task<bool> DeleteLast10Async();
}