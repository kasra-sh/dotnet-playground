using Infant.Core.Ioc;
using Infant.Core.Models.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace InfantApp.Domain;

public class ProductManager: ITransientDependency
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IServiceProvider serviceProvider)
    {
        _productRepository = serviceProvider.GetRequiredService<IProductRepository>();
    }

    public virtual async Task<bool> Insert()
    {
        return await _productRepository.InsertProductsAsync();
    }

    public virtual async Task<bool> DeleteLast10()
    {
        return await _productRepository.DeleteLast10Async();
    }
    
    public virtual async Task<int> WriteLine()
    {
        // await Task.Delay(2000);
        Console.WriteLine("My line");
        await Insert();
        return 1;
    }

    public virtual async Task<PagedResultDto<Product>> GetPaged(int size, int num, string sorting)
    {
        return await _productRepository.GetPagedList(size, num, sorting);
    }
}