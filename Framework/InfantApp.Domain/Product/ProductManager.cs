using Infant.Core.DI;

namespace InfantApp.Domain;

public class ProductManager: ITransientDependency
{
    private readonly Lazy<IProductRepository> _productRepository;

    public ProductManager()
    {
        // _productRepository = productRepository;
    }

    public virtual async Task<bool> Insert()
    {
        return await _productRepository.Value.InsertProduct();
    }
    
    public virtual async Task<int> WriteLine()
    {
        await Task.Delay(2000);
        Console.WriteLine("My line");
        return 1;
    }
}