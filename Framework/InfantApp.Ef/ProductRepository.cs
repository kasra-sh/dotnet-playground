using InfantApp.Domain;

namespace InfantApp.Ef;

public class ProductRepository : IProductRepository
{
    private readonly InfantDbContext _dbContext;

    public ProductRepository(InfantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> InsertProduct()
    {
        var productsSet = _dbContext.Set<Product>();
        
        for (int i = 0; i < 200; i++)
        {
            productsSet.Add(new Product
            {
                Name = "Abc" + Random.Shared.Next().ToString()
            });
        }
        
        await _dbContext.SaveChangesAsync();

        return true;
    }
}