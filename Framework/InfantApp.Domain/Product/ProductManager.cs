using Infant.Core.DI;
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
        return await _productRepository.InsertProduct();
    }
    
    public virtual async Task<int> WriteLine()
    {
        // await Task.Delay(2000);
        Console.WriteLine("My line");
        await Insert();
        return 1;
    }
}