using Infant.Core.Ioc;
using Infant.Core.Models.Domain;

namespace InfantApp.Domain;

public interface IProductRepository: IEfCrudRepository<Product, Guid>,ITransientDependency
{
    Task<bool> InsertProductsAsync();
    Task<bool> DeleteLast10Async();
}